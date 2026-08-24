using System.Text.Json;
using EmployeeDeskBooking.Application.Notifications;
using Microsoft.Extensions.Hosting;

namespace EmployeeDeskBooking.Infrastructure.Push;

public sealed class FilePushNotificationSender(IHostEnvironment hostEnvironment) : IPushNotificationSender
{
    public async Task SendAsync(
        string subscriptionJson,
        PushMessage message,
        CancellationToken cancellationToken = default)
    {
        var directory = Path.Combine(hostEnvironment.ContentRootPath, "App_Data", "sent-push");
        Directory.CreateDirectory(directory);

        var timestamp = DateTime.UtcNow.ToString("yyyyMMdd-HHmmss-fff");
        var safeTitle = string.Concat(message.Title.Select(ch =>
            Path.GetInvalidFileNameChars().Contains(ch) ? '_' : ch));
        var path = Path.Combine(directory, $"{timestamp}_{safeTitle}.json");

        var payload = JsonSerializer.Serialize(new
        {
            subscriptionJson,
            message.Title,
            message.Body,
        });

        await File.WriteAllTextAsync(path, payload, cancellationToken);
    }
}
