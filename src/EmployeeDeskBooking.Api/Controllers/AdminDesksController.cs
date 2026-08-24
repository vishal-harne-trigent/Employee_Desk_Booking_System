using EmployeeDeskBooking.Application.Auth;
using EmployeeDeskBooking.Application.Desks;
using EmployeeDeskBooking.Api.Models;
using EmployeeDeskBooking.Domain.Enums;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeDeskBooking.Api.Controllers;

[ApiController]
[Route("api/admin/desks")]
[Authorize(Roles = AuthRoles.Admin)]
public class AdminDesksController(IDeskService deskService) : ControllerBase
{
    [HttpGet]
    public async Task<IActionResult> GetDesks(CancellationToken cancellationToken)
    {
        var desks = await deskService.GetAllDesksAsync(cancellationToken);
        return Ok(desks.Select(desk => new AdminDeskResponse
        {
            DeskId = desk.DeskId,
            DeskNumber = desk.DeskNumber,
            Status = desk.Status.ToString(),
            CanDeactivate = desk.CanDeactivate,
            BlockingBookingCount = desk.BlockingBookingCount,
        }));
    }

    [HttpPost]
    public async Task<IActionResult> CreateDesk(
        [FromBody] CreateDeskRequest request,
        CancellationToken cancellationToken)
    {
        var result = await deskService.CreateDeskAsync(request.DeskNumber, cancellationToken);
        if (!result.Succeeded)
        {
            return Failure(result);
        }

        return Created(string.Empty, new AdminDeskResponse
        {
            DeskId = result.DeskId!.Value,
            DeskNumber = result.DeskNumber!,
            Status = DeskStatus.Active.ToString(),
            CanDeactivate = true,
            BlockingBookingCount = 0,
        });
    }

    [HttpPatch("{id:guid}")]
    public async Task<IActionResult> UpdateDesk(
        Guid id,
        [FromBody] UpdateDeskRequest request,
        CancellationToken cancellationToken)
    {
        if (!string.IsNullOrWhiteSpace(request.DeskNumber))
        {
            var updateResult = await deskService.UpdateDeskNumberAsync(
                id,
                request.DeskNumber,
                cancellationToken);

            if (!updateResult.Succeeded)
            {
                return Failure(updateResult);
            }
        }

        if (!string.IsNullOrWhiteSpace(request.Status))
        {
            if (!TryParseStatus(request.Status, out var status))
            {
                return BadRequest(new ErrorResponse { Message = "Status must be Active or Inactive." });
            }

            var statusResult = await deskService.SetDeskStatusAsync(id, status, cancellationToken);
            if (!statusResult.Succeeded)
            {
                return Failure(statusResult);
            }
        }

        var desks = await deskService.GetAllDesksAsync(cancellationToken);
        var desk = desks.FirstOrDefault(item => item.DeskId == id);
        if (desk is null)
        {
            return NotFound(new ErrorResponse { Message = "Desk not found." });
        }

        return Ok(new AdminDeskResponse
        {
            DeskId = desk.DeskId,
            DeskNumber = desk.DeskNumber,
            Status = desk.Status.ToString(),
            CanDeactivate = desk.CanDeactivate,
            BlockingBookingCount = desk.BlockingBookingCount,
        });
    }

    private IActionResult Failure(DeskMutationResult result)
    {
        var statusCode = result.FailureReason switch
        {
            DeskMutationFailureReason.InvalidNumber => StatusCodes.Status400BadRequest,
            DeskMutationFailureReason.DuplicateNumber => StatusCodes.Status422UnprocessableEntity,
            DeskMutationFailureReason.NotFound => StatusCodes.Status404NotFound,
            DeskMutationFailureReason.HasFutureBookings => StatusCodes.Status422UnprocessableEntity,
            DeskMutationFailureReason.Conflict => StatusCodes.Status409Conflict,
            _ => StatusCodes.Status400BadRequest,
        };

        return StatusCode(statusCode, new ErrorResponse
        {
            Message = AdminDeskFailureMessage.ForFailure(result),
        });
    }

    private static bool TryParseStatus(string value, out DeskStatus status)
    {
        if (string.Equals(value, "Active", StringComparison.OrdinalIgnoreCase))
        {
            status = DeskStatus.Active;
            return true;
        }

        if (string.Equals(value, "Inactive", StringComparison.OrdinalIgnoreCase))
        {
            status = DeskStatus.Inactive;
            return true;
        }

        status = default;
        return false;
    }
}

internal static class AdminDeskFailureMessage
{
    public static string ForFailure(DeskMutationResult result) =>
        result.FailureReason switch
        {
            DeskMutationFailureReason.InvalidNumber =>
                "Enter a desk number up to 32 characters.",
            DeskMutationFailureReason.DuplicateNumber =>
                "That desk number is already in use.",
            DeskMutationFailureReason.NotFound => "Desk not found.",
            DeskMutationFailureReason.HasFutureBookings =>
                result.BlockingBookingCount == 1
                    ? "This desk has 1 confirmed future booking."
                    : $"This desk has {result.BlockingBookingCount} confirmed future bookings.",
            DeskMutationFailureReason.Conflict =>
                "Could not save the desk. Try again.",
            _ => "Could not save the desk.",
        };
}
