using EmployeeDeskBooking.Application.Auth;
using EmployeeDeskBooking.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace EmployeeDeskBooking.Infrastructure.Security;

public sealed class PasswordVerifier : IPasswordVerifier
{
    private readonly PasswordHasher<User> _hasher = new();

    public string HashPassword(User user, string password) =>
        _hasher.HashPassword(user, password);

    public bool VerifyPassword(User user, string password) =>
        _hasher.VerifyHashedPassword(user, user.PasswordHash, password)
            is PasswordVerificationResult.Success
            or PasswordVerificationResult.SuccessRehashNeeded;
}
