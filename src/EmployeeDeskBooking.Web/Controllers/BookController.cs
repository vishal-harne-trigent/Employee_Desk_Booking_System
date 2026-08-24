using System.Security.Claims;
using EmployeeDeskBooking.Application.Auth;
using EmployeeDeskBooking.Application.Bookings;
using EmployeeDeskBooking.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeDeskBooking.Web.Controllers;

[Authorize(Roles = AuthRoles.Employee)]
public class BookController(
    IBookingService bookingService,
    IOfficeClock officeClock) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index(
        DateOnly? date,
        Guid? pendingDeskId,
        CancellationToken cancellationToken)
    {
        var selectedDate = date ?? officeClock.Today;
        var model = await BuildViewModelAsync(
            GetUserId(),
            selectedDate,
            pendingDeskId,
            loadAvailability: date.HasValue || pendingDeskId.HasValue,
            cancellationToken);

        if (pendingDeskId.HasValue)
        {
            var pendingDesk = model.Desks.FirstOrDefault(desk => desk.DeskId == pendingDeskId.Value);
            model.PendingDeskId = pendingDeskId;
            model.PendingDeskNumber = pendingDesk?.DeskNumber;
        }

        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CheckAvailability(
        BookDeskViewModel model,
        CancellationToken cancellationToken)
    {
        var viewModel = await BuildViewModelAsync(
            GetUserId(),
            model.SelectedDate,
            pendingDeskId: null,
            loadAvailability: true,
            cancellationToken);

        return View("Index", viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult StartBooking(DateOnly selectedDate, Guid deskId)
    {
        return RedirectToAction(nameof(Index), new { date = selectedDate, pendingDeskId = deskId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ConfirmBooking(
        DateOnly selectedDate,
        Guid deskId,
        CancellationToken cancellationToken)
    {
        var result = await bookingService.CreateBookingAsync(
            GetUserId(),
            deskId,
            selectedDate,
            cancellationToken);

        if (!result.Succeeded)
        {
            var model = await BuildViewModelAsync(
                GetUserId(),
                selectedDate,
                pendingDeskId: null,
                loadAvailability: true,
                cancellationToken);
            model.ErrorMessage = BookDeskViewModel.BookingFailureMessage(result.FailureReason);
            return View("Index", model);
        }

        var successModel = await BuildViewModelAsync(
            GetUserId(),
            selectedDate,
            pendingDeskId: null,
            loadAvailability: true,
            cancellationToken);
        successModel.SuccessMessage =
            $"Desk {result.DeskNumber} is confirmed for {selectedDate:ddd d MMM yyyy}.";
        return View("Index", successModel);
    }

    private async Task<BookDeskViewModel> BuildViewModelAsync(
        Guid userId,
        DateOnly selectedDate,
        Guid? pendingDeskId,
        bool loadAvailability,
        CancellationToken cancellationToken)
    {
        var model = new BookDeskViewModel
        {
            SelectedDate = selectedDate,
            MinDate = officeClock.Today,
            MaxDate = officeClock.Today.AddDays(BookingDateRules.MaxDaysAhead),
            ShowAvailability = loadAvailability,
        };

        if (!loadAvailability)
        {
            return model;
        }

        var availability = await bookingService.GetAvailabilityAsync(
            userId,
            selectedDate,
            cancellationToken);

        model.DateValid = availability.DateValid;
        model.DateFailure = availability.DateFailure;
        model.Desks = availability.Desks;
        model.ExistingBooking = availability.ExistingBooking;
        model.HasLoadedAvailability = true;

        if (!availability.DateValid)
        {
            model.ErrorMessage = BookDeskViewModel.DateFailureMessage(availability.DateFailure);
        }

        if (pendingDeskId.HasValue)
        {
            model.PendingDeskId = pendingDeskId;
            model.PendingDeskNumber = availability.Desks
                .FirstOrDefault(desk => desk.DeskId == pendingDeskId.Value)?.DeskNumber;
        }

        return model;
    }

    private Guid GetUserId()
    {
        var value = User.FindFirstValue(ClaimTypes.NameIdentifier)
            ?? throw new InvalidOperationException("Signed-in user id is missing.");

        return Guid.Parse(value);
    }
}
