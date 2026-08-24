using EmployeeDeskBooking.Application.Auth;
using EmployeeDeskBooking.Domain.Entities;
using EmployeeDeskBooking.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EmployeeDeskBooking.Infrastructure.Repositories;

public sealed class UserRepository(AppDbContext dbContext) : IUserRepository
{
    public async Task<User?> FindByEmailAsync(
        string email,
        CancellationToken cancellationToken = default)
    {
        var normalized = NormalizeEmail(email);
        return await dbContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(
                user => user.EmailNormalized == normalized,
                cancellationToken);
    }

    internal static string NormalizeEmail(string email) =>
        email.Trim().ToLowerInvariant();
}
