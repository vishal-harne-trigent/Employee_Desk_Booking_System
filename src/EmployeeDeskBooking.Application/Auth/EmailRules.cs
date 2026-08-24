namespace EmployeeDeskBooking.Application.Auth;

public static class EmailRules
{
    public static string Normalize(string email) =>
        email.Trim().ToLowerInvariant();
}
