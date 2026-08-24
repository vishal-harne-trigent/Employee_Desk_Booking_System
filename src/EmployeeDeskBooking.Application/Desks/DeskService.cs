using EmployeeDeskBooking.Application.Bookings;
using EmployeeDeskBooking.Domain.Entities;
using EmployeeDeskBooking.Domain.Enums;

namespace EmployeeDeskBooking.Application.Desks;

public sealed class DeskService(
    IOfficeClock officeClock,
    IDeskRepository deskRepository) : IDeskService
{
    public async Task<IReadOnlyList<DeskListItem>> GetAllDesksAsync(
        CancellationToken cancellationToken = default)
    {
        var today = officeClock.Today;
        var desks = await deskRepository.GetAllDesksAsync(cancellationToken);
        var items = new List<DeskListItem>(desks.Count);

        foreach (var desk in desks.OrderBy(d => d.DeskNumber, StringComparer.OrdinalIgnoreCase))
        {
            var blockingCount = desk.Status == DeskStatus.Active
                ? await deskRepository.CountConfirmedBookingsOnOrAfterAsync(
                    desk.Id,
                    today,
                    cancellationToken)
                : 0;

            items.Add(new DeskListItem(
                desk.Id,
                desk.DeskNumber,
                desk.Status,
                desk.Status == DeskStatus.Active && blockingCount == 0,
                blockingCount));
        }

        return items;
    }

    public async Task<DeskMutationResult> CreateDeskAsync(
        string deskNumber,
        CancellationToken cancellationToken = default)
    {
        if (!DeskNumberRules.IsValid(deskNumber))
        {
            return DeskMutationResult.Failed(DeskMutationFailureReason.InvalidNumber);
        }

        var trimmed = deskNumber.Trim();
        var normalized = DeskNumberRules.Normalize(trimmed);
        var existing = await deskRepository.FindByNormalizedNumberAsync(normalized, cancellationToken);
        if (existing is not null)
        {
            return DeskMutationResult.Failed(DeskMutationFailureReason.DuplicateNumber);
        }

        var now = officeClock.Now;
        var desk = new Desk
        {
            Id = Guid.NewGuid(),
            DeskNumber = trimmed,
            DeskNumberNormalized = normalized,
            Status = DeskStatus.Active,
            CreatedAt = now,
            UpdatedAt = now,
        };

        await deskRepository.AddAsync(desk, cancellationToken);
        var saved = await deskRepository.TrySaveChangesAsync(cancellationToken);
        return saved
            ? DeskMutationResult.Success(desk.Id, desk.DeskNumber)
            : DeskMutationResult.Failed(DeskMutationFailureReason.Conflict);
    }

    public async Task<DeskMutationResult> UpdateDeskNumberAsync(
        Guid deskId,
        string deskNumber,
        CancellationToken cancellationToken = default)
    {
        if (!DeskNumberRules.IsValid(deskNumber))
        {
            return DeskMutationResult.Failed(DeskMutationFailureReason.InvalidNumber);
        }

        var desk = await deskRepository.FindByIdTrackedAsync(deskId, cancellationToken);
        if (desk is null)
        {
            return DeskMutationResult.Failed(DeskMutationFailureReason.NotFound);
        }

        var trimmed = deskNumber.Trim();
        var normalized = DeskNumberRules.Normalize(trimmed);
        if (!string.Equals(desk.DeskNumberNormalized, normalized, StringComparison.Ordinal))
        {
            var duplicate = await deskRepository.FindByNormalizedNumberAsync(normalized, cancellationToken);
            if (duplicate is not null && duplicate.Id != deskId)
            {
                return DeskMutationResult.Failed(DeskMutationFailureReason.DuplicateNumber);
            }
        }

        desk.DeskNumber = trimmed;
        desk.DeskNumberNormalized = normalized;
        desk.UpdatedAt = officeClock.Now;

        var saved = await deskRepository.TrySaveChangesAsync(cancellationToken);
        return saved
            ? DeskMutationResult.Success(desk.Id, desk.DeskNumber)
            : DeskMutationResult.Failed(DeskMutationFailureReason.Conflict);
    }

    public async Task<DeskMutationResult> SetDeskStatusAsync(
        Guid deskId,
        DeskStatus status,
        CancellationToken cancellationToken = default)
    {
        var desk = await deskRepository.FindByIdTrackedAsync(deskId, cancellationToken);
        if (desk is null)
        {
            return DeskMutationResult.Failed(DeskMutationFailureReason.NotFound);
        }

        if (desk.Status == status)
        {
            return DeskMutationResult.Success(desk.Id, desk.DeskNumber);
        }

        if (status == DeskStatus.Inactive)
        {
            var blockingCount = await deskRepository.CountConfirmedBookingsOnOrAfterAsync(
                deskId,
                officeClock.Today,
                cancellationToken);

            if (blockingCount > 0)
            {
                return DeskMutationResult.Failed(
                    DeskMutationFailureReason.HasFutureBookings,
                    blockingCount);
            }
        }

        desk.Status = status;
        desk.UpdatedAt = officeClock.Now;

        var saved = await deskRepository.TrySaveChangesAsync(cancellationToken);
        return saved
            ? DeskMutationResult.Success(desk.Id, desk.DeskNumber)
            : DeskMutationResult.Failed(DeskMutationFailureReason.Conflict);
    }
}
