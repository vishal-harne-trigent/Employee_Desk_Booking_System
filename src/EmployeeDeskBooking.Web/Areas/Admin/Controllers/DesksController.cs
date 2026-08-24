using EmployeeDeskBooking.Application.Auth;
using EmployeeDeskBooking.Application.Desks;
using EmployeeDeskBooking.Domain.Enums;
using EmployeeDeskBooking.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeDeskBooking.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = AuthRoles.Admin)]
public class DesksController(IDeskService deskService) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index(
        bool? showAdd,
        Guid? editDeskId,
        Guid? pendingDeactivateDeskId,
        CancellationToken cancellationToken)
    {
        var desks = await deskService.GetAllDesksAsync(cancellationToken);
        var editDesk = editDeskId is null
            ? null
            : desks.FirstOrDefault(desk => desk.DeskId == editDeskId);

        return View(new ManageDesksViewModel
        {
            Desks = desks,
            ShowAddModal = showAdd == true,
            EditDeskId = editDeskId,
            EditDeskNumber = editDesk?.DeskNumber,
            PendingDeactivateDeskId = pendingDeactivateDeskId,
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult StartAdd()
    {
        return RedirectToAction(nameof(Index), new { showAdd = true });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateDesk(
        string deskNumber,
        CancellationToken cancellationToken)
    {
        var result = await deskService.CreateDeskAsync(deskNumber, cancellationToken);
        if (!result.Succeeded)
        {
            var desks = await deskService.GetAllDesksAsync(cancellationToken);
            return View("Index", new ManageDesksViewModel
            {
                Desks = desks,
                ShowAddModal = true,
                AddDeskNumber = deskNumber,
                FieldError = ManageDesksViewModel.FailureMessage(result.FailureReason),
            });
        }

        var updated = await deskService.GetAllDesksAsync(cancellationToken);
        return View("Index", new ManageDesksViewModel
        {
            Desks = updated,
            SuccessMessage = $"Desk {result.DeskNumber} was added.",
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult StartEdit(Guid deskId)
    {
        return RedirectToAction(nameof(Index), new { editDeskId = deskId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SaveEdit(
        Guid deskId,
        string deskNumber,
        CancellationToken cancellationToken)
    {
        var result = await deskService.UpdateDeskNumberAsync(deskId, deskNumber, cancellationToken);
        if (!result.Succeeded)
        {
            var desks = await deskService.GetAllDesksAsync(cancellationToken);
            return View("Index", new ManageDesksViewModel
            {
                Desks = desks,
                EditDeskId = deskId,
                EditDeskNumber = deskNumber,
                FieldError = ManageDesksViewModel.FailureMessage(result.FailureReason),
            });
        }

        var updated = await deskService.GetAllDesksAsync(cancellationToken);
        return View("Index", new ManageDesksViewModel
        {
            Desks = updated,
            SuccessMessage = $"Desk number updated to {result.DeskNumber}.",
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult StartDeactivate(Guid deskId)
    {
        return RedirectToAction(nameof(Index), new { pendingDeactivateDeskId = deskId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ConfirmDeactivate(
        Guid deskId,
        CancellationToken cancellationToken)
    {
        var result = await deskService.SetDeskStatusAsync(
            deskId,
            DeskStatus.Inactive,
            cancellationToken);

        var desks = await deskService.GetAllDesksAsync(cancellationToken);

        if (!result.Succeeded)
        {
            return View("Index", new ManageDesksViewModel
            {
                Desks = desks,
                ErrorMessage = ManageDesksViewModel.FailureMessage(
                    result.FailureReason,
                    result.BlockingBookingCount),
            });
        }

        return View("Index", new ManageDesksViewModel
        {
            Desks = desks,
            SuccessMessage = $"Desk {result.DeskNumber} was deactivated.",
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Activate(
        Guid deskId,
        CancellationToken cancellationToken)
    {
        var result = await deskService.SetDeskStatusAsync(
            deskId,
            DeskStatus.Active,
            cancellationToken);

        var desks = await deskService.GetAllDesksAsync(cancellationToken);

        if (!result.Succeeded)
        {
            return View("Index", new ManageDesksViewModel
            {
                Desks = desks,
                ErrorMessage = ManageDesksViewModel.FailureMessage(result.FailureReason),
            });
        }

        return View("Index", new ManageDesksViewModel
        {
            Desks = desks,
            SuccessMessage = $"Desk {result.DeskNumber} is active again.",
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> AttemptDeactivate(
        Guid deskId,
        CancellationToken cancellationToken)
    {
        var desks = await deskService.GetAllDesksAsync(cancellationToken);
        var desk = desks.FirstOrDefault(item => item.DeskId == deskId);

        if (desk is null)
        {
            return View("Index", new ManageDesksViewModel
            {
                Desks = desks,
                ErrorMessage = ManageDesksViewModel.FailureMessage(DeskMutationFailureReason.NotFound),
            });
        }

        if (desk.CanDeactivate)
        {
            return RedirectToAction(nameof(Index), new { pendingDeactivateDeskId = deskId });
        }

        return View("Index", new ManageDesksViewModel
        {
            Desks = desks,
            ErrorMessage = ManageDesksViewModel.FailureMessage(
                DeskMutationFailureReason.HasFutureBookings,
                desk.BlockingBookingCount),
        });
    }
}
