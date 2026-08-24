namespace EmployeeDeskBooking.Application.Notifications;

internal static class BookingEmailTemplates
{
    public static string FormatBookingDate(DateOnly date) =>
        date.ToString("d MMM yyyy");

    public static (string Subject, string HtmlBody) Confirmation(
        string deskNumber,
        DateOnly bookingDate)
    {
        var formattedDate = FormatBookingDate(bookingDate);
        var subject = $"Desk booking confirmed — {deskNumber} on {formattedDate}";
        var body =
            $"<html><body><p>Your desk <strong>{deskNumber}</strong> is confirmed for <strong>{formattedDate}</strong>.</p></body></html>";

        return (subject, body);
    }

    public static (string Subject, string HtmlBody) Cancellation(
        string deskNumber,
        DateOnly bookingDate)
    {
        var formattedDate = FormatBookingDate(bookingDate);
        var subject = $"Desk booking cancelled — {deskNumber} on {formattedDate}";
        var body =
            $"<html><body><p>Your booking for desk <strong>{deskNumber}</strong> on <strong>{formattedDate}</strong> has been cancelled.</p></body></html>";

        return (subject, body);
    }

    public static (string Subject, string HtmlBody) Reminder(
        string deskNumber,
        DateOnly bookingDate)
    {
        var formattedDate = FormatBookingDate(bookingDate);
        var subject = $"Desk booking reminder — {deskNumber} on {formattedDate}";
        var body =
            $"<html><body><p>Reminder: you have desk <strong>{deskNumber}</strong> booked for <strong>{formattedDate}</strong>.</p></body></html>";

        return (subject, body);
    }
}
