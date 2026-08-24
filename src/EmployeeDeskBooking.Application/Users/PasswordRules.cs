using System.Security.Cryptography;

namespace EmployeeDeskBooking.Application.Users;

public static class PasswordRules
{
    public const int MinLength = 8;

    public static bool IsValid(string? password) =>
        !string.IsNullOrWhiteSpace(password) && password.Length >= MinLength;

    public static string GenerateTemporaryPassword()
    {
        const string upper = "ABCDEFGHJKLMNPQRSTUVWXYZ";
        const string lower = "abcdefghijkmnopqrstuvwxyz";
        const string digits = "23456789";
        const string special = "!@#$%&*";

        Span<char> password = stackalloc char[12];
        password[0] = upper[RandomNumberGenerator.GetInt32(upper.Length)];
        password[1] = lower[RandomNumberGenerator.GetInt32(lower.Length)];
        password[2] = digits[RandomNumberGenerator.GetInt32(digits.Length)];
        password[3] = special[RandomNumberGenerator.GetInt32(special.Length)];

        var all = upper + lower + digits + special;
        for (var i = 4; i < password.Length; i++)
        {
            password[i] = all[RandomNumberGenerator.GetInt32(all.Length)];
        }

        return new string(password);
    }
}
