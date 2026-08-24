using EmployeeDeskBooking.Domain.Entities;

namespace EmployeeDeskBooking.Application.Desks;

public interface IDeskRepository
{
    Task<IReadOnlyList<Desk>> GetActiveDesksAsync(CancellationToken cancellationToken = default);

    Task<IReadOnlyList<Desk>> GetAllDesksAsync(CancellationToken cancellationToken = default);

    Task<Desk?> FindByIdAsync(Guid deskId, CancellationToken cancellationToken = default);

    Task<Desk?> FindByIdTrackedAsync(Guid deskId, CancellationToken cancellationToken = default);

    Task<Desk?> FindByNormalizedNumberAsync(
        string normalizedDeskNumber,
        CancellationToken cancellationToken = default);

    Task AddAsync(Desk desk, CancellationToken cancellationToken = default);

    Task<int> CountConfirmedBookingsOnOrAfterAsync(
        Guid deskId,
        DateOnly fromDate,
        CancellationToken cancellationToken = default);

    Task<bool> TrySaveChangesAsync(CancellationToken cancellationToken = default);
}
