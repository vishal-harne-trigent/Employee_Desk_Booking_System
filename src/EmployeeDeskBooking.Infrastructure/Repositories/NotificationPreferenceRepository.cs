using EmployeeDeskBooking.Application.Notifications;
using EmployeeDeskBooking.Domain.Entities;
using EmployeeDeskBooking.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EmployeeDeskBooking.Infrastructure.Repositories;

public sealed class NotificationPreferenceRepository(AppDbContext dbContext)
    : INotificationPreferenceRepository
{
    public async Task<NotificationPreference?> FindByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default) =>
        await dbContext.NotificationPreferences
            .AsNoTracking()
            .FirstOrDefaultAsync(preference => preference.UserId == userId, cancellationToken);

    public async Task<NotificationPreference> GetOrCreateAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var existing = await dbContext.NotificationPreferences
            .FirstOrDefaultAsync(preference => preference.UserId == userId, cancellationToken);

        if (existing is not null)
        {
            return existing;
        }

        var created = new NotificationPreference
        {
            UserId = userId,
            PushOptIn = false,
            UpdatedAt = DateTimeOffset.UtcNow,
        };

        dbContext.NotificationPreferences.Add(created);
        return created;
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
}
