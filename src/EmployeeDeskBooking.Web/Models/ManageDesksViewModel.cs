using EmployeeDeskBooking.Application.Desks;
using EmployeeDeskBooking.Domain.Enums;

namespace EmployeeDeskBooking.Web.Models;

public class ManageDesksViewModel
{
    public IReadOnlyList<DeskListItem> Desks { get; set; } = Array.Empty<DeskListItem>();

    public bool ShowAddModal { get; set; }

    public Guid? EditDeskId { get; set; }

    public string? EditDeskNumber { get; set; }

    public Guid? PendingDeactivateDeskId { get; set; }

    public DeskListItem? PendingDeactivateDesk =>
        PendingDeactivateDeskId is null
            ? null
            : Desks.FirstOrDefault(desk => desk.DeskId == PendingDeactivateDeskId);

    public string? AddDeskNumber { get; set; }

    public string? FieldError { get; set; }

    public string? SuccessMessage { get; set; }

    public string? ErrorMessage { get; set; }

    public static string StatusLabel(DeskStatus status) =>
        status switch
        {
            DeskStatus.Active => "Active",
            DeskStatus.Inactive => "Inactive",
            _ => status.ToString(),
        };

    public static string StatusCssClass(DeskStatus status) =>
        status switch
        {
            DeskStatus.Active => "pill-confirmed",
            DeskStatus.Inactive => "pill-cancelled",
            _ => "pill-booked",
        };

    public static string FailureMessage(DeskMutationFailureReason reason, int blockingCount = 0) =>
        reason switch
        {
            DeskMutationFailureReason.InvalidNumber =>
                "Enter a desk number up to 32 characters.",
            DeskMutationFailureReason.DuplicateNumber =>
                "That desk number is already in use.",
            DeskMutationFailureReason.NotFound => "Desk not found.",
            DeskMutationFailureReason.HasFutureBookings =>
                blockingCount == 1
                    ? "This desk has 1 confirmed booking from today onward. Cancel it in All bookings before deactivating."
                    : $"This desk has {blockingCount} confirmed bookings from today onward. Cancel them in All bookings before deactivating.",
            DeskMutationFailureReason.Conflict =>
                "Could not save the desk. Please try again.",
            _ => "Could not save the desk.",
        };
}
