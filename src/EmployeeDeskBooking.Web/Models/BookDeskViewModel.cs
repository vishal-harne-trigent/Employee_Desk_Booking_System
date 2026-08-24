using System.ComponentModel.DataAnnotations;
using EmployeeDeskBooking.Application.Bookings;

namespace EmployeeDeskBooking.Web.Models;

public class BookDeskViewModel
{
    [Display(Name = "Office date")]
    [DataType(DataType.Date)]
    public DateOnly SelectedDate { get; set; }

    public DateOnly MinDate { get; set; }

    public DateOnly MaxDate { get; set; }

    public bool DateValid { get; set; } = true;

    public DateValidationFailure DateFailure { get; set; }

    public IReadOnlyList<DeskAvailabilityItem> Desks { get; set; } =
        Array.Empty<DeskAvailabilityItem>();

    public EmployeeBookingSummary? ExistingBooking { get; set; }

    public Guid? PendingDeskId { get; set; }

    public string? PendingDeskNumber { get; set; }

    public string? SuccessMessage { get; set; }

    public string? ErrorMessage { get; set; }

    public bool ShowAvailability { get; set; }

    public bool HasLoadedAvailability { get; set; }

    public static string DateFailureMessage(DateValidationFailure failure) =>
        failure switch
        {
            DateValidationFailure.BeforeToday =>
                "That date is in the past. Choose today or a future working day.",
            DateValidationFailure.AfterWindow =>
                "That date is outside the 30-day booking window.",
            DateValidationFailure.Weekend =>
                "Bookings are only available Monday through Friday.",
            _ => "Please choose a valid working day.",
        };

    public static string BookingFailureMessage(CreateBookingFailureReason reason) =>
        reason switch
        {
            CreateBookingFailureReason.EmployeeAlreadyBooked =>
                "You already have a desk booked for this date.",
            CreateBookingFailureReason.DeskInactive =>
                "That desk is not available for booking.",
            CreateBookingFailureReason.DeskAlreadyBooked =>
                "That desk was just booked by someone else. Refresh and try another desk.",
            CreateBookingFailureReason.DeskNotFound =>
                "Desk not found.",
            CreateBookingFailureReason.InvalidDate =>
                "The selected date is not valid for booking.",
            CreateBookingFailureReason.Conflict =>
                "Could not complete the booking due to a conflict. Please refresh and try again.",
            _ => "Could not complete the booking. Please try again.",
        };
}
