using EmployeeDeskBooking.Application.Auth;
using EmployeeDeskBooking.Application.Users;
using EmployeeDeskBooking.Domain.Enums;

namespace EmployeeDeskBooking.Web.Models;

public class ManageUsersViewModel
{
    public IReadOnlyList<UserListItem> Users { get; set; } = Array.Empty<UserListItem>();

    public bool ShowAddModal { get; set; }

    public Guid? EditUserId { get; set; }

    public string? EditName { get; set; }

    public string? EditEmail { get; set; }

    public string? EditRole { get; set; }

    public string? AddName { get; set; }

    public string? AddEmail { get; set; }

    public string? AddRole { get; set; } = AuthRoles.Employee;

    public string? AddPassword { get; set; }

    public Guid? PendingDeactivateUserId { get; set; }

    public UserListItem? PendingDeactivateUser =>
        PendingDeactivateUserId is null
            ? null
            : Users.FirstOrDefault(user => user.UserId == PendingDeactivateUserId);

    public string? ResetPasswordForName { get; set; }

    public string? ResetPasswordPlaintext { get; set; }

    public string? FieldError { get; set; }

    public string? SuccessMessage { get; set; }

    public string? ErrorMessage { get; set; }

    public static string RoleLabel(UserRole role) =>
        role switch
        {
            UserRole.Admin => "Admin",
            UserRole.Employee => "Employee",
            _ => role.ToString(),
        };

    public static string StatusLabel(bool isActive) =>
        isActive ? "Active" : "Inactive";

    public static string StatusCssClass(bool isActive) =>
        isActive ? "pill-confirmed" : "pill-cancelled";

    public static UserRole? ParseRole(string? value) =>
        value switch
        {
            "Admin" => UserRole.Admin,
            "Employee" => UserRole.Employee,
            _ => null,
        };

    public static string FailureMessage(UserMutationFailureReason reason) =>
        reason switch
        {
            UserMutationFailureReason.InvalidEmail => "Enter a valid email address.",
            UserMutationFailureReason.InvalidName => "Enter the user's name.",
            UserMutationFailureReason.InvalidPassword =>
                "Password must be at least 8 characters.",
            UserMutationFailureReason.InvalidRole => "Choose Admin or Employee.",
            UserMutationFailureReason.DuplicateEmail =>
                "That email is already assigned to another user.",
            UserMutationFailureReason.NotFound => "User not found.",
            UserMutationFailureReason.LastActiveAdmin =>
                "Cannot remove or deactivate the last active Admin.",
            UserMutationFailureReason.AlreadyInactive =>
                "This account is already inactive.",
            UserMutationFailureReason.Conflict =>
                "Could not save the user. Please try again.",
            _ => "Could not save the user.",
        };
}
