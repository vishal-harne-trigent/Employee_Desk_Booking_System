using System.Text.Json;
using EmployeeDeskBooking.Application.Notifications;
using Microsoft.Extensions.Configuration;
using WebPush;

namespace EmployeeDeskBooking.Infrastructure.Push;

public sealed class WebPushNotificationSender(IConfiguration configuration) : IPushNotificationSender
{
    public Task SendAsync(
        string subscriptionJson,
        PushMessage message,
        CancellationToken cancellationToken = default)
    {
        var subject = configuration["Vapid:Subject"]
            ?? throw new InvalidOperationException("Vapid:Subject is not configured.");
        var publicKey = configuration["Vapid:PublicKey"]
            ?? throw new InvalidOperationException("Vapid:PublicKey is not configured.");
        var privateKey = configuration["Vapid:PrivateKey"]
            ?? throw new InvalidOperationException("Vapid:PrivateKey is not configured.");

        var subscription = JsonSerializer.Deserialize<PushSubscriptionDto>(
            subscriptionJson,
            new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
            ?? throw new InvalidOperationException("Push subscription JSON is invalid.");

        var pushSubscription = new PushSubscription(
            subscription.Endpoint,
            subscription.Keys.P256dh,
            subscription.Keys.Auth);

        var payload = JsonSerializer.Serialize(new
        {
            title = message.Title,
            body = message.Body,
        });

        var client = new WebPushClient();
        var vapid = new VapidDetails(subject, publicKey, privateKey);
        return client.SendNotificationAsync(pushSubscription, payload, vapid);
    }

    private sealed class PushSubscriptionDto
    {
        public string Endpoint { get; set; } = string.Empty;

        public PushKeys Keys { get; set; } = new();
    }

    private sealed class PushKeys
    {
        public string P256dh { get; set; } = string.Empty;

        public string Auth { get; set; } = string.Empty;
    }
}
