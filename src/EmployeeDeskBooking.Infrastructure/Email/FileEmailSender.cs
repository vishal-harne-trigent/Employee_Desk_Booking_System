using EmployeeDeskBooking.Application.Notifications;
using Microsoft.Extensions.Hosting;

namespace EmployeeDeskBooking.Infrastructure.Email;

public sealed class FileEmailSender(IHostEnvironment hostEnvironment) : IEmailSender
{
    public async Task SendAsync(EmailMessage message, CancellationToken cancellationToken = default)
    {
        var directory = Path.Combine(hostEnvironment.ContentRootPath, "App_Data", "sent-emails");
        Directory.CreateDirectory(directory);

        var timestamp = DateTime.UtcNow.ToString("yyyyMMdd-HHmmss-fff");
        var safeSubject = string.Concat(message.Subject.Select(ch =>
            Path.GetInvalidFileNameChars().Contains(ch) ? '_' : ch));
        var fileName = $"{timestamp}_{message.To}_{safeSubject}.html";
        var path = Path.Combine(directory, fileName);

        var html =
            $"<!DOCTYPE html><html><head><meta charset=\"utf-8\" />" +
            $"<title>{message.Subject}</title></head><body>" +
            $"<p><strong>To:</strong> {message.To}</p>" +
            $"<p><strong>Subject:</strong> {message.Subject}</p>" +
            $"<hr />{message.HtmlBody}</body></html>";

        await File.WriteAllTextAsync(path, html, cancellationToken);
    }
}
