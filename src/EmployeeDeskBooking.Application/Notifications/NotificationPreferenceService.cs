using EmployeeDeskBooking.Application.Bookings;

namespace EmployeeDeskBooking.Application.Notifications;

public sealed class NotificationPreferenceService(
    IOfficeClock officeClock,
    INotificationPreferenceRepository preferenceRepository) : INotificationPreferenceService
{
    public async Task<NotificationPreferenceDto> GetPreferencesAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var preference = await preferenceRepository.FindByUserIdAsync(userId, cancellationToken);
        return new NotificationPreferenceDto(
            preference?.PushOptIn ?? false,
            !string.IsNullOrWhiteSpace(preference?.PushSubscription));
    }

    public async Task EnablePushAsync(
        Guid userId,
        string subscriptionJson,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(subscriptionJson))
        {
            throw new ArgumentException("Push subscription is required.", nameof(subscriptionJson));
        }

        var preference = await preferenceRepository.GetOrCreateAsync(userId, cancellationToken);
        preference.PushOptIn = true;
        preference.PushSubscription = subscriptionJson;
        preference.UpdatedAt = officeClock.Now;
        await preferenceRepository.TrySaveChangesAsync(cancellationToken);
    }

    public async Task DisablePushAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var preference = await preferenceRepository.GetOrCreateAsync(userId, cancellationToken);
        preference.PushOptIn = false;
        preference.PushSubscription = null;
        preference.UpdatedAt = officeClock.Now;
        await preferenceRepository.TrySaveChangesAsync(cancellationToken);
    }

    public async Task<(bool ShouldSend, string? SubscriptionJson)> GetPushSubscriptionAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var preference = await preferenceRepository.FindByUserIdAsync(userId, cancellationToken);
        if (preference is null || !preference.PushOptIn ||
            string.IsNullOrWhiteSpace(preference.PushSubscription))
        {
            return (false, null);
        }

        return (true, preference.PushSubscription);
    }
}
