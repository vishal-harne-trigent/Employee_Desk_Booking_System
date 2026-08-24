using EmployeeDeskBooking.Domain.Enums;

namespace EmployeeDeskBooking.Application.Desks;

public interface IDeskService
{
    Task<IReadOnlyList<DeskListItem>> GetAllDesksAsync(CancellationToken cancellationToken = default);

    Task<DeskMutationResult> CreateDeskAsync(
        string deskNumber,
        CancellationToken cancellationToken = default);

    Task<DeskMutationResult> UpdateDeskNumberAsync(
        Guid deskId,
        string deskNumber,
        CancellationToken cancellationToken = default);

    Task<DeskMutationResult> SetDeskStatusAsync(
        Guid deskId,
        DeskStatus status,
        CancellationToken cancellationToken = default);
}
