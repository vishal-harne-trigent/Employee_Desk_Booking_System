using EmployeeDeskBooking.Domain.Enums;

namespace EmployeeDeskBooking.Application.Bookings;

public enum CreateBookingFailureReason
{
    None = 0,
    InvalidDate = 1,
    EmployeeAlreadyBooked = 2,
    DeskNotFound = 3,
    DeskInactive = 4,
    DeskAlreadyBooked = 5,
    Conflict = 6,
}

public enum CancelBookingFailureReason
{
    None = 0,
    NotFound = 1,
    NotCancellable = 2,
    Conflict = 3,
}

public sealed record DeskAvailabilityItem(
    Guid DeskId,
    string DeskNumber,
    bool IsAvailable);

public sealed record EmployeeBookingSummary(
    Guid BookingId,
    string DeskNumber);

public sealed record AvailabilityResult(
    bool DateValid,
    DateValidationFailure DateFailure,
    IReadOnlyList<DeskAvailabilityItem> Desks,
    EmployeeBookingSummary? ExistingBooking)
{
    public static AvailabilityResult InvalidDate(DateValidationFailure failure) =>
        new(false, failure, Array.Empty<DeskAvailabilityItem>(), null);
}

public sealed record CreateBookingResult(
    bool Succeeded,
    Guid? BookingId,
    string? DeskNumber,
    CreateBookingFailureReason FailureReason)
{
    public static CreateBookingResult Success(Guid bookingId, string deskNumber) =>
        new(true, bookingId, deskNumber, CreateBookingFailureReason.None);

    public static CreateBookingResult Failed(CreateBookingFailureReason reason) =>
        new(false, null, null, reason);
}

public sealed record MyBookingItem(
    Guid BookingId,
    DateOnly BookingDate,
    string DeskNumber,
    BookingStatus Status,
    bool CanCancel);

public sealed record AdminBookingFilters(
    DateOnly? BookingDate,
    BookingStatus? Status);

public sealed record AdminBookingItem(
    Guid BookingId,
    DateOnly BookingDate,
    string DeskNumber,
    string EmployeeEmail,
    BookingStatus Status,
    bool CanCancel);

public sealed record CancelBookingResult(
    bool Succeeded,
    CancelBookingFailureReason FailureReason)
{
    public static CancelBookingResult Success() =>
        new(true, CancelBookingFailureReason.None);

    public static CancelBookingResult Failed(CancelBookingFailureReason reason) =>
        new(false, reason);
}
