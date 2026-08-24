namespace EmployeeDeskBooking.Api.Models;

public sealed class LoginRequest
{
    public string Email { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;
}

public sealed class LoginResponse
{
    public string Token { get; set; } = string.Empty;

    public int ExpiresInMinutes { get; set; }

    public string Email { get; set; } = string.Empty;

    public string Name { get; set; } = string.Empty;

    public string Role { get; set; } = string.Empty;
}

public sealed class ErrorResponse
{
    public string Message { get; set; } = string.Empty;
}
