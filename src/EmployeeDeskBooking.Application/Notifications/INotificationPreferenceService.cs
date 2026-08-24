namespace EmployeeDeskBooking.Application.Notifications;

public sealed record NotificationPreferenceDto(
    bool PushOptIn,
    bool HasSubscription);

public interface INotificationPreferenceService
{
    Task<NotificationPreferenceDto> GetPreferencesAsync(
        Guid userId,
        CancellationToken cancellationToken = default);

    Task EnablePushAsync(
        Guid userId,
        string subscriptionJson,
        CancellationToken cancellationToken = default);

    Task DisablePushAsync(
        Guid userId,
        CancellationToken cancellationToken = default);

    Task<(bool ShouldSend, string? SubscriptionJson)> GetPushSubscriptionAsync(
        Guid userId,
        CancellationToken cancellationToken = default);
}
