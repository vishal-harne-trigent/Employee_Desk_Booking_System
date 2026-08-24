using EmployeeDeskBooking.Application.Auth;
using EmployeeDeskBooking.Domain.Entities;
using EmployeeDeskBooking.Domain.Enums;
using EmployeeDeskBooking.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

namespace EmployeeDeskBooking.Infrastructure.Data;

public static class DbInitializer
{
    public static async Task SeedAsync(AppDbContext dbContext, IPasswordVerifier passwordVerifier)
    {
        await SeedUsersAsync(dbContext, passwordVerifier);
        await SeedDesksAsync(dbContext);
        await SeedSampleBookingsAsync(dbContext);
        await SeedEmployeeDemoBookingsAsync(dbContext);
    }

    private static async Task SeedUsersAsync(
        AppDbContext dbContext,
        IPasswordVerifier passwordVerifier)
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

    private static async Task SeedDesksAsync(AppDbContext dbContext)
    {
        if (await dbContext.Desks.AnyAsync())
        {
            return;
        }

        var now = DateTimeOffset.UtcNow;
        var desks = new[]
        {
            CreateDesk("A-01", DeskStatus.Active, now),
            CreateDesk("A-02", DeskStatus.Active, now),
            CreateDesk("B-01", DeskStatus.Active, now),
            CreateDesk("B-02", DeskStatus.Inactive, now),
        };

        dbContext.Desks.AddRange(desks);
        await dbContext.SaveChangesAsync();
    }

    private static async Task SeedSampleBookingsAsync(AppDbContext dbContext)
    {
        if (await dbContext.Bookings.AnyAsync())
        {
            return;
        }

        var admin = await dbContext.Users
            .FirstOrDefaultAsync(user => user.EmailNormalized == "admin@trigent.com");

        var deskA02 = await dbContext.Desks
            .FirstOrDefaultAsync(desk => desk.DeskNumberNormalized == "a-02");

        if (admin is null || deskA02 is null)
        {
            return;
        }

        var now = DateTimeOffset.UtcNow;
        var sampleDate = DateOnly.FromDateTime(now.UtcDateTime.AddDays(1));

        dbContext.Bookings.Add(new Booking
        {
            Id = Guid.NewGuid(),
            UserId = admin.Id,
            DeskId = deskA02.Id,
            BookingDate = sampleDate,
            Status = BookingStatus.Confirmed,
            CreatedAt = now,
            UpdatedAt = now,
        });

        await dbContext.SaveChangesAsync();
    }

    private static async Task SeedEmployeeDemoBookingsAsync(AppDbContext dbContext)
    {
        var employee = await dbContext.Users
            .FirstOrDefaultAsync(user => user.EmailNormalized == "vishal_h@trigent.com");

        if (employee is null)
        {
            return;
        }

        if (await dbContext.Bookings.AnyAsync(booking => booking.UserId == employee.Id))
        {
            return;
        }

        var deskA01 = await dbContext.Desks
            .FirstOrDefaultAsync(desk => desk.DeskNumberNormalized == "a-01");
        var deskB01 = await dbContext.Desks
            .FirstOrDefaultAsync(desk => desk.DeskNumberNormalized == "b-01");

        if (deskA01 is null || deskB01 is null)
        {
            return;
        }

        var now = DateTimeOffset.UtcNow;
        var today = DateOnly.FromDateTime(now.UtcDateTime);
        var futureDate = today.AddDays(3);
        var pastDate = today.AddDays(-5);
        var cancelledDate = today.AddDays(-10);

        dbContext.Bookings.AddRange(
            new Booking
            {
                Id = Guid.NewGuid(),
                UserId = employee.Id,
                DeskId = deskA01.Id,
                BookingDate = futureDate,
                Status = BookingStatus.Confirmed,
                CreatedAt = now,
                UpdatedAt = now,
            },
            new Booking
            {
                Id = Guid.NewGuid(),
                UserId = employee.Id,
                DeskId = deskB01.Id,
                BookingDate = pastDate,
                Status = BookingStatus.Completed,
                CompletedAt = now,
                CreatedAt = now.AddDays(-6),
                UpdatedAt = now,
            },
            new Booking
            {
                Id = Guid.NewGuid(),
                UserId = employee.Id,
                DeskId = deskA01.Id,
                BookingDate = cancelledDate,
                Status = BookingStatus.Cancelled,
                CancelledAt = now.AddDays(-11),
                CancelledById = employee.Id,
                CreatedAt = now.AddDays(-12),
                UpdatedAt = now.AddDays(-11),
            });

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

    private static Desk CreateDesk(string deskNumber, DeskStatus status, DateTimeOffset now) =>
        new()
        {
            Id = Guid.NewGuid(),
            DeskNumber = deskNumber,
            DeskNumberNormalized = NormalizeDeskNumber(deskNumber),
            Status = status,
            CreatedAt = now,
            UpdatedAt = now,
        };

    internal static string NormalizeDeskNumber(string deskNumber) =>
        deskNumber.Trim().ToLowerInvariant();
}
