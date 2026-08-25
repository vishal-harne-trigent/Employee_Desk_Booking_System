namespace EmployeeDeskBooking.Application.Bookings;

public interface IBookingService
{
    Task<AvailabilityResult> GetAvailabilityAsync(
        Guid userId,
        DateOnly bookingDate,
        CancellationToken cancellationToken = default);

    Task<CreateBookingResult> CreateBookingAsync(
        Guid userId,
        Guid deskId,
        DateOnly bookingDate,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<MyBookingItem>> GetMyBookingsAsync(
        Guid userId,
        CancellationToken cancellationToken = default);

    Task<CancelBookingResult> CancelBookingAsync(
        Guid userId,
        Guid bookingId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<AdminBookingItem>> GetAllBookingsAsync(
        AdminBookingFilters filters,
        CancellationToken cancellationToken = default);

    Task<CancelBookingResult> CancelBookingAsAdminAsync(
        Guid adminUserId,
        Guid bookingId,
        CancellationToken cancellationToken = default);

    Task<int> CompletePastBookingsAsync(CancellationToken cancellationToken = default);
}
