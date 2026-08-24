namespace EmployeeDeskBooking.Application.Desks;

public static class DeskNumberRules
{
    public const int MaxLength = 32;

    public static string Normalize(string deskNumber) =>
        deskNumber.Trim().ToLowerInvariant();

    public static bool IsValid(string? deskNumber) =>
        !string.IsNullOrWhiteSpace(deskNumber) &&
        deskNumber.Trim().Length <= MaxLength;
}
