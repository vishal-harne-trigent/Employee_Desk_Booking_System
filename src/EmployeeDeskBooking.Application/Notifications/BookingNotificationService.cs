using EmployeeDeskBooking.Application.Bookings;
using EmployeeDeskBooking.Domain.Enums;

namespace EmployeeDeskBooking.Application.Notifications;

public sealed class BookingNotificationService(
    IOfficeClock officeClock,
    IBookingRepository bookingRepository,
    IEmailSender emailSender,
    IEmailDeliveryRepository emailDeliveryRepository,
    INotificationPreferenceService notificationPreferenceService,
    IPushNotificationSender pushNotificationSender) : IBookingNotificationService
{
    public Task SendConfirmationAsync(
        Guid bookingId,
        CancellationToken cancellationToken = default) =>
        SendBookingEventAsync(
            bookingId,
            BookingEmailType.Confirmation,
            booking => BookingEmailTemplates.Confirmation(
                booking.Desk.DeskNumber,
                booking.BookingDate),
            booking => BookingPushTemplates.Confirmation(
                booking.Desk.DeskNumber,
                booking.BookingDate),
            cancellationToken);

    public Task SendCancellationAsync(
        Guid bookingId,
        CancellationToken cancellationToken = default) =>
        SendBookingEventAsync(
            bookingId,
            BookingEmailType.Cancellation,
            booking => BookingEmailTemplates.Cancellation(
                booking.Desk.DeskNumber,
                booking.BookingDate),
            booking => BookingPushTemplates.Cancellation(
                booking.Desk.DeskNumber,
                booking.BookingDate),
            cancellationToken);

    public async Task SendDueRemindersAsync(CancellationToken cancellationToken = default)
    {
        var tomorrow = officeClock.Today.AddDays(1);
        if (tomorrow.DayOfWeek is DayOfWeek.Saturday or DayOfWeek.Sunday)
        {
            return;
        }

        var bookings = await bookingRepository.GetConfirmedBookingsForDateAsync(
            tomorrow,
            cancellationToken);

        foreach (var booking in bookings)
        {
            if (await emailDeliveryRepository.HasReminderSentAsync(booking.Id, cancellationToken))
            {
                continue;
            }

            var (subject, body) = BookingEmailTemplates.Reminder(
                booking.Desk.DeskNumber,
                booking.BookingDate);

            var sent = await TrySendEmailAsync(
                booking.Id,
                booking.UserId,
                booking.User.Email,
                BookingEmailType.Reminder,
                subject,
                body,
                cancellationToken);

            if (sent)
            {
                await emailDeliveryRepository.AddReminderAsync(
                    booking.Id,
                    officeClock.Now,
                    cancellationToken);
                await emailDeliveryRepository.TrySaveChangesAsync(cancellationToken);
            }
        }
    }

    private async Task SendBookingEventAsync(
        Guid bookingId,
        BookingEmailType emailType,
        Func<Domain.Entities.Booking, (string Subject, string HtmlBody)> buildEmail,
        Func<Domain.Entities.Booking, PushMessage> buildPush,
        CancellationToken cancellationToken)
    {
        var booking = await bookingRepository.GetByIdAsync(bookingId, cancellationToken);
        if (booking is null)
        {
            return;
        }

        var (subject, body) = buildEmail(booking);
        await TrySendEmailAsync(
            booking.Id,
            booking.UserId,
            booking.User.Email,
            emailType,
            subject,
            body,
            cancellationToken);

        await TrySendPushAsync(booking.UserId, buildPush(booking), cancellationToken);
    }

    private async Task TrySendPushAsync(
        Guid userId,
        PushMessage message,
        CancellationToken cancellationToken)
    {
        var (shouldSend, subscriptionJson) =
            await notificationPreferenceService.GetPushSubscriptionAsync(userId, cancellationToken);

        if (!shouldSend || subscriptionJson is null)
        {
            return;
        }

        try
        {
            await pushNotificationSender.SendAsync(subscriptionJson, message, cancellationToken);
        }
        catch
        {
            // Push failure must not block booking; email already sent.
        }
    }

    private async Task<bool> TrySendEmailAsync(
        Guid bookingId,
        Guid userId,
        string recipient,
        BookingEmailType emailType,
        string subject,
        string htmlBody,
        CancellationToken cancellationToken)
    {
        var now = officeClock.Now;

        try
        {
            await emailSender.SendAsync(
                new EmailMessage(recipient, subject, htmlBody),
                cancellationToken);

            await emailDeliveryRepository.AddLogAsync(
                bookingId,
                userId,
                emailType,
                recipient,
                EmailDeliveryStatus.Sent,
                errorMessage: null,
                now,
                cancellationToken);
            await emailDeliveryRepository.TrySaveChangesAsync(cancellationToken);
            return true;
        }
        catch (Exception ex)
        {
            await emailDeliveryRepository.AddLogAsync(
                bookingId,
                userId,
                emailType,
                recipient,
                EmailDeliveryStatus.Failed,
                ex.Message,
                now,
                cancellationToken);
            await emailDeliveryRepository.TrySaveChangesAsync(cancellationToken);
            return false;
        }
    }
}
