using EmployeeDeskBooking.Application.Bookings;
using EmployeeDeskBooking.Domain.Enums;

namespace EmployeeDeskBooking.Web.Models;

public class AdminBookingsViewModel
{
    public IReadOnlyList<AdminBookingItem> Bookings { get; set; } = Array.Empty<AdminBookingItem>();

    public DateOnly? FilterDate { get; set; }

    public string FilterStatus { get; set; } = "All";

    public bool HasActiveFilters =>
        FilterDate is not null || !string.Equals(FilterStatus, "All", StringComparison.OrdinalIgnoreCase);

    public Guid? PendingCancelBookingId { get; set; }

    public AdminBookingItem? PendingCancelBooking =>
        PendingCancelBookingId is null
            ? null
            : Bookings.FirstOrDefault(booking => booking.BookingId == PendingCancelBookingId);

    public string? SuccessMessage { get; set; }

    public string? ErrorMessage { get; set; }

    public static string StatusLabel(BookingStatus status) =>
        MyBookingsViewModel.StatusLabel(status);

    public static string StatusCssClass(BookingStatus status) =>
        MyBookingsViewModel.StatusCssClass(status);

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

    public static BookingStatus? ParseStatusFilter(string? value) =>
        value switch
        {
            "Confirmed" => BookingStatus.Confirmed,
            "Cancelled" => BookingStatus.Cancelled,
            "Completed" => BookingStatus.Completed,
            _ => null,
        };
}
