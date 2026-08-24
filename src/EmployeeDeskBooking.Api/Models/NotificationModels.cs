namespace EmployeeDeskBooking.Api.Models;

public sealed class NotificationPreferencesResponse
{
    public bool PushOptIn { get; set; }

    public bool HasSubscription { get; set; }

    public string? VapidPublicKey { get; set; }
}

public sealed class UpdateNotificationPreferencesRequest
{
    public bool? PushOptIn { get; set; }
}

public sealed class PushSubscriptionRequest
{
    public string SubscriptionJson { get; set; } = string.Empty;
}
