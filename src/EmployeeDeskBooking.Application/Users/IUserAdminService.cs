using EmployeeDeskBooking.Domain.Enums;

namespace EmployeeDeskBooking.Application.Users;

public interface IUserAdminService
{
    Task<IReadOnlyList<UserListItem>> GetAllUsersAsync(CancellationToken cancellationToken = default);

    Task<UserMutationResult> CreateUserAsync(
        string email,
        string name,
        UserRole role,
        string password,
        CancellationToken cancellationToken = default);

    Task<UserMutationResult> UpdateUserAsync(
        Guid userId,
        string email,
        string name,
        UserRole role,
        CancellationToken cancellationToken = default);

    Task<UserMutationResult> DeactivateUserAsync(
        Guid userId,
        CancellationToken cancellationToken = default);

    Task<ResetPasswordResult> ResetPasswordAsync(
        Guid userId,
        CancellationToken cancellationToken = default);
}
