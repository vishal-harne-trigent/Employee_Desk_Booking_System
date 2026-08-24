namespace EmployeeDeskBooking.Application.Notifications;

internal static class BookingPushTemplates
{
    public static PushMessage Confirmation(string deskNumber, DateOnly bookingDate) =>
        new(
            "Desk booking confirmed",
            $"Desk {deskNumber} on {BookingEmailTemplates.FormatBookingDate(bookingDate)}");

    public static PushMessage Cancellation(string deskNumber, DateOnly bookingDate) =>
        new(
            "Desk booking cancelled",
            $"Desk {deskNumber} on {BookingEmailTemplates.FormatBookingDate(bookingDate)}");
}
