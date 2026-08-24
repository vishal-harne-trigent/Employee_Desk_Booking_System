using EmployeeDeskBooking.Application.Auth;
using EmployeeDeskBooking.Domain.Entities;
using EmployeeDeskBooking.Domain.Enums;
using EmployeeDeskBooking.Infrastructure.Data;
using EmployeeDeskBooking.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace EmployeeDeskBooking.Infrastructure.Data;

public static class DbInitializer
{
    public static async Task SeedAsync(AppDbContext dbContext, IPasswordVerifier passwordVerifier)
    {
        if (await dbContext.Users.AnyAsync())
        {
            return;
        }

        var now = DateTimeOffset.UtcNow;
        var users = new[]
        {
            CreateUser(
                "admin@trigent.com",
                "System Admin",
                UserRole.Admin,
                "Password1!",
                isActive: true,
                now,
                passwordVerifier),
            CreateUser(
                "vishal_h@trigent.com",
                "Vishal Harne",
                UserRole.Employee,
                "Password1!",
                isActive: true,
                now,
                passwordVerifier),
        };

        dbContext.Users.AddRange(users);
        await dbContext.SaveChangesAsync();
    }

    private static User CreateUser(
        string email,
        string name,
        UserRole role,
        string password,
        bool isActive,
        DateTimeOffset now,
        IPasswordVerifier passwordVerifier)
    {
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = email,
            EmailNormalized = UserRepository.NormalizeEmail(email),
            Name = name,
            Role = role,
            IsActive = isActive,
            CreatedAt = now,
            UpdatedAt = now,
        };

        user.PasswordHash = passwordVerifier.HashPassword(user, password);
        return user;
    }
}
