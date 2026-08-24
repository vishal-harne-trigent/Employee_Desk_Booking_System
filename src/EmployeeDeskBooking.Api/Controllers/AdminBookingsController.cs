using System.Security.Claims;
using EmployeeDeskBooking.Application.Auth;
using EmployeeDeskBooking.Application.Bookings;
using EmployeeDeskBooking.Api.Models;
using EmployeeDeskBooking.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeDeskBooking.Api.Controllers;

[ApiController]
[Route("api/admin/bookings")]
[Authorize(Roles = AuthRoles.Admin)]
public class AdminBookingsController(IBookingService bookingService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetAllBookings(
        [FromQuery] DateOnly? date,
        [FromQuery] string? status,
        CancellationToken cancellationToken)
    {
        var bookings = await bookingService.GetAllBookingsAsync(
            new AdminBookingFilters(date, ParseStatus(status)),
            cancellationToken);

        return Ok(bookings.Select(booking => new AdminBookingResponse
        {
            BookingId = booking.BookingId,
            BookingDate = booking.BookingDate,
            DeskNumber = booking.DeskNumber,
            EmployeeEmail = booking.EmployeeEmail,
            Status = booking.Status.ToString(),
            CanCancel = booking.CanCancel,
        }));
    }

    [HttpPost("{id:guid}/cancel")]
    public async Task<IActionResult> CancelBooking(
        Guid id,
        CancellationToken cancellationToken)
    {
        var result = await bookingService.CancelBookingAsAdminAsync(
            GetUserId(),
            id,
            cancellationToken);

        if (!result.Succeeded)
        {
            var statusCode = result.FailureReason switch
            {
                CancelBookingFailureReason.NotFound => StatusCodes.Status404NotFound,
                CancelBookingFailureReason.NotCancellable => StatusCodes.Status422UnprocessableEntity,
                CancelBookingFailureReason.Conflict => StatusCodes.Status409Conflict,
                _ => StatusCodes.Status400BadRequest,
            };

            return StatusCode(statusCode, new ErrorResponse
            {
                Message = AdminBookingsFailureMessage.ForCancelFailure(result.FailureReason),
            });
        }

        return NoContent();
    }

    private static BookingStatus? ParseStatus(string? value) =>
        value switch
        {
            "Confirmed" => BookingStatus.Confirmed,
            "Cancelled" => BookingStatus.Cancelled,
            "Completed" => BookingStatus.Completed,
            _ => null,
        };

    private Guid GetUserId()
    {
        var value = User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new InvalidOperationException("Signed-in user id is missing.");

        return Guid.Parse(value);
    }
}

internal static class AdminBookingsFailureMessage
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
