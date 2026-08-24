using EmployeeDeskBooking.Application.Auth;
using EmployeeDeskBooking.Application.Users;
using EmployeeDeskBooking.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace EmployeeDeskBooking.Web.Areas.Admin.Controllers;

[Area("Admin")]
[Authorize(Roles = AuthRoles.Admin)]
public class UsersController(IUserAdminService userAdminService) : Controller
{
    [HttpGet]
    public async Task<IActionResult> Index(
        bool? showAdd,
        Guid? editUserId,
        Guid? pendingDeactivateUserId,
        CancellationToken cancellationToken)
    {
        var users = await userAdminService.GetAllUsersAsync(cancellationToken);
        var editUser = editUserId is null
            ? null
            : users.FirstOrDefault(user => user.UserId == editUserId);

        return View(new ManageUsersViewModel
        {
            Users = users,
            ShowAddModal = showAdd == true,
            EditUserId = editUserId,
            EditName = editUser?.Name,
            EditEmail = editUser?.Email,
            EditRole = editUser is null ? null : ManageUsersViewModel.RoleLabel(editUser.Role),
            PendingDeactivateUserId = pendingDeactivateUserId,
            ResetPasswordForName = TempData["ResetPasswordForName"] as string,
            ResetPasswordPlaintext = TempData["ResetPasswordPlaintext"] as string,
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
    public async Task<IActionResult> CreateUser(
        string name,
        string email,
        string role,
        string password,
        CancellationToken cancellationToken)
    {
        var parsedRole = ManageUsersViewModel.ParseRole(role);
        if (parsedRole is null)
        {
            return await FailureViewAsync(
                cancellationToken,
                showAdd: true,
                addName: name,
                addEmail: email,
                addRole: role,
                addPassword: password,
                fieldError: ManageUsersViewModel.FailureMessage(UserMutationFailureReason.InvalidRole));
        }

        var result = await userAdminService.CreateUserAsync(
            email,
            name,
            parsedRole.Value,
            password,
            cancellationToken);

        if (!result.Succeeded)
        {
            return await FailureViewAsync(
                cancellationToken,
                showAdd: true,
                addName: name,
                addEmail: email,
                addRole: role,
                addPassword: password,
                fieldError: ManageUsersViewModel.FailureMessage(result.FailureReason));
        }

        var users = await userAdminService.GetAllUsersAsync(cancellationToken);
        return View("Index", new ManageUsersViewModel
        {
            Users = users,
            SuccessMessage = "User account was created.",
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult StartEdit(Guid userId)
    {
        return RedirectToAction(nameof(Index), new { editUserId = userId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> SaveEdit(
        Guid userId,
        string name,
        string email,
        string role,
        CancellationToken cancellationToken)
    {
        var parsedRole = ManageUsersViewModel.ParseRole(role);
        if (parsedRole is null)
        {
            return await FailureViewAsync(
                cancellationToken,
                editUserId: userId,
                editName: name,
                editEmail: email,
                editRole: role,
                fieldError: ManageUsersViewModel.FailureMessage(UserMutationFailureReason.InvalidRole));
        }

        var result = await userAdminService.UpdateUserAsync(
            userId,
            email,
            name,
            parsedRole.Value,
            cancellationToken);

        if (!result.Succeeded)
        {
            return await FailureViewAsync(
                cancellationToken,
                editUserId: userId,
                editName: name,
                editEmail: email,
                editRole: role,
                fieldError: ManageUsersViewModel.FailureMessage(result.FailureReason));
        }

        var users = await userAdminService.GetAllUsersAsync(cancellationToken);
        return View("Index", new ManageUsersViewModel
        {
            Users = users,
            SuccessMessage = "User profile was updated.",
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public IActionResult StartDeactivate(Guid userId)
    {
        return RedirectToAction(nameof(Index), new { pendingDeactivateUserId = userId });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ConfirmDeactivate(
        Guid userId,
        CancellationToken cancellationToken)
    {
        var result = await userAdminService.DeactivateUserAsync(userId, cancellationToken);
        var users = await userAdminService.GetAllUsersAsync(cancellationToken);

        if (!result.Succeeded)
        {
            return View("Index", new ManageUsersViewModel
            {
                Users = users,
                ErrorMessage = ManageUsersViewModel.FailureMessage(result.FailureReason),
            });
        }

        return View("Index", new ManageUsersViewModel
        {
            Users = users,
            SuccessMessage = "User account was deactivated.",
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ResetPassword(
        Guid userId,
        CancellationToken cancellationToken)
    {
        var usersBefore = await userAdminService.GetAllUsersAsync(cancellationToken);
        var user = usersBefore.FirstOrDefault(item => item.UserId == userId);
        if (user is null)
        {
            return View("Index", new ManageUsersViewModel
            {
                Users = usersBefore,
                ErrorMessage = ManageUsersViewModel.FailureMessage(UserMutationFailureReason.NotFound),
            });
        }

        var result = await userAdminService.ResetPasswordAsync(userId, cancellationToken);
        var users = await userAdminService.GetAllUsersAsync(cancellationToken);

        if (!result.Succeeded)
        {
            return View("Index", new ManageUsersViewModel
            {
                Users = users,
                ErrorMessage = ManageUsersViewModel.FailureMessage(result.FailureReason),
            });
        }

        TempData["ResetPasswordForName"] = user.Name;
        TempData["ResetPasswordPlaintext"] = result.PlaintextPassword;
        return RedirectToAction(nameof(Index));
    }

    private async Task<IActionResult> FailureViewAsync(
        CancellationToken cancellationToken,
        bool showAdd = false,
        Guid? editUserId = null,
        string? addName = null,
        string? addEmail = null,
        string? addRole = null,
        string? addPassword = null,
        string? editName = null,
        string? editEmail = null,
        string? editRole = null,
        string? fieldError = null)
    {
        var users = await userAdminService.GetAllUsersAsync(cancellationToken);
        return View("Index", new ManageUsersViewModel
        {
            Users = users,
            ShowAddModal = showAdd,
            EditUserId = editUserId,
            AddName = addName,
            AddEmail = addEmail,
            AddRole = addRole,
            AddPassword = addPassword,
            EditName = editName,
            EditEmail = editEmail,
            EditRole = editRole,
            FieldError = fieldError,
        });
    }
}
