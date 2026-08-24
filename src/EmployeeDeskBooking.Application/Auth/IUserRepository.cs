using EmployeeDeskBooking.Domain.Entities;

namespace EmployeeDeskBooking.Application.Auth;

public interface IUserRepository
{
    Task<User?> FindByEmailAsync(string email, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<User>> GetAllAsync(CancellationToken cancellationToken = default);

    Task<User?> FindByIdAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<User?> FindByIdTrackedAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<User?> FindByNormalizedEmailAsync(
        string normalizedEmail,
        CancellationToken cancellationToken = default);

    Task<int> CountActiveAdminsAsync(
        Guid? excludeUserId = null,
        CancellationToken cancellationToken = default);

    Task AddAsync(User user, CancellationToken cancellationToken = default);

    Task<bool> TrySaveChangesAsync(CancellationToken cancellationToken = default);
}
