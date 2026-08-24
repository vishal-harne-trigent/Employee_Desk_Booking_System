using EmployeeDeskBooking.Domain.Entities;

namespace EmployeeDeskBooking.Application.Notifications;

public interface INotificationPreferenceRepository
{
    Task<NotificationPreference?> FindByUserIdAsync(
        Guid userId,
        CancellationToken cancellationToken = default);

    Task<NotificationPreference> GetOrCreateAsync(
        Guid userId,
        CancellationToken cancellationToken = default);

    Task<bool> TrySaveChangesAsync(CancellationToken cancellationToken = default);
}
