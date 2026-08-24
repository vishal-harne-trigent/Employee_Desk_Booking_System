using EmployeeDeskBooking.Application.Bookings;
using EmployeeDeskBooking.Domain.Enums;

namespace EmployeeDeskBooking.Web.Models;

public class MyBookingsViewModel
{
    public IReadOnlyList<MyBookingItem> Bookings { get; set; } = Array.Empty<MyBookingItem>();

    public Guid? PendingCancelBookingId { get; set; }

    public MyBookingItem? PendingCancelBooking =>
        PendingCancelBookingId is null
            ? null
            : Bookings.FirstOrDefault(booking => booking.BookingId == PendingCancelBookingId);

    public string? SuccessMessage { get; set; }

    public string? ErrorMessage { get; set; }

    public static string StatusLabel(BookingStatus status) =>
        status switch
        {
            BookingStatus.Confirmed => "Confirmed",
            BookingStatus.Cancelled => "Cancelled",
            BookingStatus.Completed => "Completed",
            _ => status.ToString(),
        };

    public static string StatusCssClass(BookingStatus status) =>
        status switch
        {
            BookingStatus.Confirmed => "pill-confirmed",
            BookingStatus.Cancelled => "pill-cancelled",
            BookingStatus.Completed => "pill-completed",
            _ => "pill-booked",
        };

    public static string CancelFailureMessage(CancelBookingFailureReason reason) =>
        reason switch
        {
            CancelBookingFailureReason.NotFound => "Booking not found.",
            CancelBookingFailureReason.NotCancellable =>
                "This booking cannot be cancelled.",
            CancelBookingFailureReason.Conflict =>
                "Could not cancel the booking. Please try again.",
            _ => "Could not cancel the booking.",
        };
}
