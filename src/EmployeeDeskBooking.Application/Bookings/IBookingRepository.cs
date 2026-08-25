using EmployeeDeskBooking.Domain.Entities;
using EmployeeDeskBooking.Domain.Enums;

namespace EmployeeDeskBooking.Application.Bookings;

public interface IBookingRepository
{
    Task<Booking?> GetConfirmedForUserOnDateAsync(
        Guid userId,
        DateOnly bookingDate,
        CancellationToken cancellationToken = default);

    Task<HashSet<Guid>> GetBookedDeskIdsForDateAsync(
        DateOnly bookingDate,
        CancellationToken cancellationToken = default);

    Task AddAsync(Booking booking, CancellationToken cancellationToken = default);

    Task<Booking?> GetByIdForUserAsync(
        Guid bookingId,
        Guid userId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Booking>> GetAllForUserAsync(
        Guid userId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Booking>> GetAllAsync(
        DateOnly? bookingDate,
        BookingStatus? status,
        CancellationToken cancellationToken = default);

    Task<Booking?> GetByIdAsync(
        Guid bookingId,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Booking>> GetConfirmedBookingsForDateAsync(
        DateOnly bookingDate,
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Booking>> GetConfirmedBookingsBeforeDateAsync(
        DateOnly beforeDate,
        CancellationToken cancellationToken = default);

    Task<bool> TrySaveChangesAsync(CancellationToken cancellationToken = default);
}
