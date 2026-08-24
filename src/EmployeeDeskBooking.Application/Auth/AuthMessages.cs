using EmployeeDeskBooking.Domain.Enums;

namespace EmployeeDeskBooking.Application.Auth;

public static class AuthMessages
{
    public const string InvalidCredentials =
        "Invalid email or password. Please try again.";

    public const string DeactivatedAccount =
        "Your account has been deactivated. Contact your administrator.";
}

public static class AuthRoles
{
    public const string Employee = nameof(UserRole.Employee);

    public const string Admin = nameof(UserRole.Admin);
}
