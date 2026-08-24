using EmployeeDeskBooking.Application.Notifications;
using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Configuration;
using MimeKit;

namespace EmployeeDeskBooking.Infrastructure.Email;

public sealed class SmtpEmailSender(IConfiguration configuration) : IEmailSender
{
    public async Task SendAsync(EmailMessage message, CancellationToken cancellationToken = default)
    {
        var mime = new MimeMessage();
        mime.From.Add(new MailboxAddress(
            configuration["Smtp:FromName"] ?? "Desk Booking",
            configuration["Smtp:FromAddress"]
                ?? throw new InvalidOperationException("Smtp:FromAddress is not configured.")));
        mime.To.Add(MailboxAddress.Parse(message.To));
        mime.Subject = message.Subject;
        mime.Body = new TextPart("html") { Text = message.HtmlBody };

        using var client = new SmtpClient();
        var host = configuration["Smtp:Host"]
            ?? throw new InvalidOperationException("Smtp:Host is not configured.");
        var port = int.TryParse(configuration["Smtp:Port"], out var parsedPort) ? parsedPort : 587;

        await client.ConnectAsync(host, port, SecureSocketOptions.StartTls, cancellationToken);

        var username = configuration["Smtp:Username"];
        var password = configuration["Smtp:Password"];
        if (!string.IsNullOrWhiteSpace(username) && !string.IsNullOrWhiteSpace(password))
        {
            await client.AuthenticateAsync(username, password, cancellationToken);
        }

        await client.SendAsync(mime, cancellationToken);
        await client.DisconnectAsync(true, cancellationToken);
    }
}
