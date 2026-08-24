using System.Security.Claims;
using EmployeeDeskBooking.Application.Auth;
using EmployeeDeskBooking.Application.Bookings;
using EmployeeDeskBooking.Api.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeDeskBooking.Api.Controllers;

[ApiController]
[Route("api/bookings")]
[Authorize(Roles = AuthRoles.Employee)]
public class BookingsController(IBookingService bookingService) : ControllerBase
{
    [HttpGet("availability")]
    public async Task<IActionResult> GetAvailability(
        [FromQuery] DateOnly date,
        CancellationToken cancellationToken)
    {
        var result = await bookingService.GetAvailabilityAsync(
            GetUserId(),
            date,
            cancellationToken);

        if (!result.DateValid)
        {
            return BadRequest(new ErrorResponse
            {
                Message = BookDeskFailureMessage.ForDateValidation(result.DateFailure),
            });
        }

        return Ok(new AvailabilityResponse
        {
            Date = date,
            Desks = result.Desks.Select(desk => new DeskAvailabilityResponse
            {
                DeskId = desk.DeskId,
                DeskNumber = desk.DeskNumber,
                IsAvailable = desk.IsAvailable,
            }).ToList(),
            ExistingBooking = result.ExistingBooking is null
                ? null
                : new ExistingBookingResponse
                {
                    BookingId = result.ExistingBooking.BookingId,
                    DeskNumber = result.ExistingBooking.DeskNumber,
                },
        });
    }

    [HttpPost]
    public async Task<IActionResult> CreateBooking(
        [FromBody] CreateBookingRequest request,
        CancellationToken cancellationToken)
    {
        var result = await bookingService.CreateBookingAsync(
            GetUserId(),
            request.DeskId,
            request.BookingDate,
            cancellationToken);

        if (!result.Succeeded)
        {
            var status = result.FailureReason switch
            {
                CreateBookingFailureReason.DeskNotFound => StatusCodes.Status404NotFound,
                CreateBookingFailureReason.InvalidDate => StatusCodes.Status400BadRequest,
                CreateBookingFailureReason.EmployeeAlreadyBooked => StatusCodes.Status409Conflict,
                CreateBookingFailureReason.DeskInactive => StatusCodes.Status422UnprocessableEntity,
                CreateBookingFailureReason.DeskAlreadyBooked => StatusCodes.Status409Conflict,
                CreateBookingFailureReason.Conflict => StatusCodes.Status409Conflict,
                _ => StatusCodes.Status400BadRequest,
            };

            return StatusCode(status, new ErrorResponse
            {
                Message = BookDeskFailureMessage.ForCreateFailure(result.FailureReason),
            });
        }

        return Created(string.Empty, new CreateBookingResponse
        {
            BookingId = result.BookingId!.Value,
            DeskNumber = result.DeskNumber!,
            BookingDate = request.BookingDate,
        });
    }

    [HttpGet("mine")]
    public async Task<IActionResult> GetMyBookings(CancellationToken cancellationToken)
    {
        var bookings = await bookingService.GetMyBookingsAsync(GetUserId(), cancellationToken);
        return Ok(bookings.Select(booking => new MyBookingResponse
        {
            BookingId = booking.BookingId,
            BookingDate = booking.BookingDate,
            DeskNumber = booking.DeskNumber,
            Status = booking.Status.ToString(),
            CanCancel = booking.CanCancel,
        }));
    }

    [HttpPost("{id:guid}/cancel")]
    public async Task<IActionResult> CancelBooking(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await bookingService.CancelBookingAsync(
            GetUserId(),
            id,
            cancellationToken);

        if (!result.Succeeded)
        {
            var status = result.FailureReason switch
            {
                CancelBookingFailureReason.NotFound => StatusCodes.Status404NotFound,
                CancelBookingFailureReason.NotCancellable => StatusCodes.Status422UnprocessableEntity,
                CancelBookingFailureReason.Conflict => StatusCodes.Status409Conflict,
                _ => StatusCodes.Status400BadRequest,
            };

            return StatusCode(status, new ErrorResponse
            {
                Message = MyBookingsFailureMessage.ForCancelFailure(result.FailureReason),
            });
        }

        return NoContent();
    }

    private Guid GetUserId()
    {
        var value = User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new InvalidOperationException("Signed-in user id is missing.");

        return Guid.Parse(value);
    }
}

internal static class BookDeskFailureMessage
{
    public static string ForDateValidation(DateValidationFailure failure) =>
        failure switch
        {
            DateValidationFailure.BeforeToday => "Date is in the past.",
            DateValidationFailure.AfterWindow => "Date is outside the 30-day booking window.",
            DateValidationFailure.Weekend => "Weekend dates are not bookable.",
            _ => "Invalid booking date.",
        };

    public static string ForCreateFailure(CreateBookingFailureReason reason) =>
        reason switch
        {
            CreateBookingFailureReason.EmployeeAlreadyBooked =>
                "You already have a booking on this date.",
            CreateBookingFailureReason.DeskInactive => "Desk is inactive.",
            CreateBookingFailureReason.DeskAlreadyBooked => "Desk is already booked.",
            CreateBookingFailureReason.DeskNotFound => "Desk not found.",
            CreateBookingFailureReason.InvalidDate => "Invalid booking date.",
            CreateBookingFailureReason.Conflict => "Booking conflict. Try again.",
            _ => "Could not create booking.",
        };
}

internal static class MyBookingsFailureMessage
{
    public static string ForCancelFailure(CancelBookingFailureReason reason) =>
        reason switch
        {
            CancelBookingFailureReason.NotFound => "Booking not found.",
            CancelBookingFailureReason.NotCancellable => "Booking cannot be cancelled.",
            CancelBookingFailureReason.Conflict => "Could not cancel booking. Try again.",
            _ => "Could not cancel booking.",
        };
}
