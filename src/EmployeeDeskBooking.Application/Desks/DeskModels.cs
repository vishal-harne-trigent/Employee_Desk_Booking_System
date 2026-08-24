using EmployeeDeskBooking.Domain.Enums;

namespace EmployeeDeskBooking.Application.Desks;

public enum DeskMutationFailureReason
{
    None = 0,
    InvalidNumber = 1,
    DuplicateNumber = 2,
    NotFound = 3,
    HasFutureBookings = 4,
    Conflict = 5,
}

public sealed record DeskListItem(
    Guid DeskId,
    string DeskNumber,
    DeskStatus Status,
    bool CanDeactivate,
    int BlockingBookingCount);

public sealed record DeskMutationResult(
    bool Succeeded,
    Guid? DeskId,
    string? DeskNumber,
    DeskMutationFailureReason FailureReason,
    int BlockingBookingCount = 0)
{
    public static DeskMutationResult Success(Guid deskId, string deskNumber) =>
        new(true, deskId, deskNumber, DeskMutationFailureReason.None);

    public static DeskMutationResult Failed(
        DeskMutationFailureReason reason,
        int blockingBookingCount = 0) =>
        new(false, null, null, reason, blockingBookingCount);
}
