namespace EmployeeDeskBooking.Application.Notifications;

public sealed record PushMessage(
    string Title,
    string Body);

public interface IPushNotificationSender
{
    Task SendAsync(
        string subscriptionJson,
        PushMessage message,
        CancellationToken cancellationToken = default);
}
