namespace EmployeeDeskBooking.Api.Models;

public sealed class CreateUserRequest
{
    public string Name { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Role { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;
}

public sealed class UpdateUserRequest
{
    public string? Name { get; set; }

    public string? Email { get; set; }

    public string? Role { get; set; }

    public bool? Deactivate { get; set; }
}

public sealed class AdminUserResponse
{
    public Guid UserId { get; set; }

    public string Name { get; set; } = string.Empty;

    public string Email { get; set; } = string.Empty;

    public string Role { get; set; } = string.Empty;

    public bool IsActive { get; set; }

    public bool IsLastActiveAdmin { get; set; }
}

public sealed class ResetPasswordResponse
{
    public string Password { get; set; } = string.Empty;
}
