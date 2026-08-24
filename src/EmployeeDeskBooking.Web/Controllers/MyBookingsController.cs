using System.Security.Claims;
using EmployeeDeskBooking.Application.Auth;
using EmployeeDeskBooking.Application.Bookings;
using EmployeeDeskBooking.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeDeskBooking.Web.Controllers;

[Authorize(Roles = AuthRoles.Employee)]
public class MyBookingsController(IBookingService bookingService) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index(Guid? pendingCancelId, CancellationToken cancellationToken)
    {
        var bookings = await bookingService.GetMyBookingsAsync(GetUserId(), cancellationToken);
        return View(new MyBookingsViewModel
        {
            Bookings = bookings,
            PendingCancelBookingId = pendingCancelId,
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult StartCancel(Guid bookingId)
    {
        return RedirectToAction(nameof(Index), new { pendingCancelId = bookingId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ConfirmCancel(
        Guid bookingId,
        CancellationToken cancellationToken)
    {
        var result = await bookingService.CancelBookingAsync(
            GetUserId(),
            bookingId,
            cancellationToken);

        if (!result.Succeeded)
        {
            var bookings = await bookingService.GetMyBookingsAsync(GetUserId(), cancellationToken);
            return View("Index", new MyBookingsViewModel
            {
                Bookings = bookings,
                ErrorMessage = MyBookingsViewModel.CancelFailureMessage(result.FailureReason),
            });
        }

        var updated = await bookingService.GetMyBookingsAsync(GetUserId(), cancellationToken);
        return View("Index", new MyBookingsViewModel
        {
            Bookings = updated,
            SuccessMessage = "Your booking was cancelled.",
        });
    }

    private Guid GetUserId()
    {
        var value = User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new InvalidOperationException("Signed-in user id is missing.");

        return Guid.Parse(value);
    }
}
