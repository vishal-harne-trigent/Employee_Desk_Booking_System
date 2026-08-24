using EmployeeDeskBooking.Application.Auth;
using EmployeeDeskBooking.Domain.Entities;
using EmployeeDeskBooking.Domain.Enums;
using EmployeeDeskBooking.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EmployeeDeskBooking.Infrastructure.Repositories;

public sealed class UserRepository(AppDbContext dbContext) : IUserRepository
{
    public async Task<User?> FindByEmailAsync(
        string email,
        CancellationToken cancellationToken = default)
    {
        var normalized = EmailRules.Normalize(email);
        return await dbContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(
                user => user.EmailNormalized == normalized,
                cancellationToken);
    }

    public async Task<IReadOnlyList<User>> GetAllAsync(
        CancellationToken cancellationToken = default) =>
        await dbContext.Users
            .AsNoTracking()
            .ToListAsync(cancellationToken);

    public async Task<User?> FindByIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default) =>
        await dbContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(user => user.Id == userId, cancellationToken);

    public async Task<User?> FindByIdTrackedAsync(
        Guid userId,
        CancellationToken cancellationToken = default) =>
        await dbContext.Users
            .FirstOrDefaultAsync(user => user.Id == userId, cancellationToken);

    public async Task<User?> FindByNormalizedEmailAsync(
        string normalizedEmail,
        CancellationToken cancellationToken = default) =>
        await dbContext.Users
            .AsNoTracking()
            .FirstOrDefaultAsync(
                user => user.EmailNormalized == normalizedEmail,
                cancellationToken);

    public async Task<int> CountActiveAdminsAsync(
        Guid? excludeUserId = null,
        CancellationToken cancellationToken = default)
    {
        var query = dbContext.Users
            .AsNoTracking()
            .Where(user => user.Role == UserRole.Admin && user.IsActive);

        if (excludeUserId is not null)
        {
            query = query.Where(user => user.Id != excludeUserId);
        }

        return await query.CountAsync(cancellationToken);
    }

    public Task AddAsync(User user, CancellationToken cancellationToken = default)
    {
        dbContext.Users.Add(user);
        return Task.CompletedTask;
    }

    public async Task<bool> TrySaveChangesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await dbContext.SaveChangesAsync(cancellationToken);
            return true;
        }
        catch (DbUpdateException)
        {
            return false;
        }
    }

    public static string NormalizeEmail(string email) =>
        EmailRules.Normalize(email);
}
