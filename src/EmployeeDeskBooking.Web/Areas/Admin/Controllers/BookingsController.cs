using System.Security.Claims;
using EmployeeDeskBooking.Application.Auth;
using EmployeeDeskBooking.Application.Bookings;
using EmployeeDeskBooking.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeDeskBooking.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = AuthRoles.Admin)]
public class BookingsController(IBookingService bookingService) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index(
        DateOnly? filterDate,
        string? filterStatus,
        Guid? pendingCancelId,
        CancellationToken cancellationToken)
    {
        var status = AdminBookingsViewModel.ParseStatusFilter(filterStatus);
        var bookings = await bookingService.GetAllBookingsAsync(
            new AdminBookingFilters(filterDate, status),
            cancellationToken);

        return View(new AdminBookingsViewModel
        {
            Bookings = bookings,
            FilterDate = filterDate,
            FilterStatus = string.IsNullOrWhiteSpace(filterStatus) ? "All" : filterStatus,
            PendingCancelBookingId = pendingCancelId,
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult ApplyFilters(DateOnly? filterDate, string? filterStatus)
    {
        return RedirectToAction(nameof(Index), new
        {
            filterDate,
            filterStatus = string.IsNullOrWhiteSpace(filterStatus) ? "All" : filterStatus,
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult ClearFilters()
    {
        return RedirectToAction(nameof(Index));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult StartCancel(
        Guid bookingId,
        DateOnly? filterDate,
        string? filterStatus)
    {
        return RedirectToAction(nameof(Index), new
        {
            filterDate,
            filterStatus = string.IsNullOrWhiteSpace(filterStatus) ? "All" : filterStatus,
            pendingCancelId = bookingId,
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ConfirmCancel(
        Guid bookingId,
        DateOnly? filterDate,
        string? filterStatus,
        CancellationToken cancellationToken)
    {
        var status = AdminBookingsViewModel.ParseStatusFilter(filterStatus);
        var filters = new AdminBookingFilters(filterDate, status);
        var result = await bookingService.CancelBookingAsAdminAsync(
            GetUserId(),
            bookingId,
            cancellationToken);

        if (!result.Succeeded)
        {
            var bookings = await bookingService.GetAllBookingsAsync(filters, cancellationToken);
            return View("Index", new AdminBookingsViewModel
            {
                Bookings = bookings,
                FilterDate = filterDate,
                FilterStatus = string.IsNullOrWhiteSpace(filterStatus) ? "All" : filterStatus,
                PendingCancelBookingId = bookingId,
                ErrorMessage = AdminBookingsViewModel.CancelFailureMessage(result.FailureReason),
            });
        }

        var updated = await bookingService.GetAllBookingsAsync(filters, cancellationToken);
        return View("Index", new AdminBookingsViewModel
        {
            Bookings = updated,
            FilterDate = filterDate,
            FilterStatus = string.IsNullOrWhiteSpace(filterStatus) ? "All" : filterStatus,
            SuccessMessage = "The booking was cancelled.",
        });
    }

    private Guid GetUserId()
    {
        var value = User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new InvalidOperationException("Signed-in user id is missing.");

        return Guid.Parse(value);
    }
}
