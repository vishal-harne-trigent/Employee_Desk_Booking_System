using EmployeeDeskBooking.Application.Auth;
using EmployeeDeskBooking.Domain.Entities;
using EmployeeDeskBooking.Domain.Enums;

namespace EmployeeDeskBooking.Application.Users;

public sealed class UserAdminService(
    IUserRepository userRepository,
    IPasswordVerifier passwordVerifier) : IUserAdminService
{
    public async Task<IReadOnlyList<UserListItem>> GetAllUsersAsync(
        CancellationToken cancellationToken = default)
    {
        var users = await userRepository.GetAllAsync(cancellationToken);
        var activeAdminCount = users.Count(user =>
            user.Role == UserRole.Admin && user.IsActive);

        return users
            .OrderBy(user => user.Name, StringComparer.OrdinalIgnoreCase)
            .Select(user => new UserListItem(
                user.Id,
                user.Name,
                user.Email,
                user.Role,
                user.IsActive,
                user.Role == UserRole.Admin &&
                user.IsActive &&
                activeAdminCount == 1))
            .ToList();
    }

    public async Task<UserMutationResult> CreateUserAsync(
        string email,
        string name,
        UserRole role,
        string password,
        CancellationToken cancellationToken = default)
    {
        var validation = ValidateProfile(email, name, role);
        if (validation is not null)
        {
            return validation;
        }

        if (!PasswordRules.IsValid(password))
        {
            return UserMutationResult.Failed(UserMutationFailureReason.InvalidPassword);
        }

        var normalizedEmail = EmailRules.Normalize(email);
        var duplicate = await userRepository.FindByNormalizedEmailAsync(normalizedEmail, cancellationToken);
        if (duplicate is not null)
        {
            return UserMutationResult.Failed(UserMutationFailureReason.DuplicateEmail);
        }

        var now = DateTimeOffset.UtcNow;
        var user = new User
        {
            Id = Guid.NewGuid(),
            Email = email.Trim(),
            EmailNormalized = normalizedEmail,
            Name = name.Trim(),
            Role = role,
            IsActive = true,
            CreatedAt = now,
            UpdatedAt = now,
        };

        user.PasswordHash = passwordVerifier.HashPassword(user, password);
        await userRepository.AddAsync(user, cancellationToken);

        var saved = await userRepository.TrySaveChangesAsync(cancellationToken);
        return saved
            ? UserMutationResult.Success(user.Id)
            : UserMutationResult.Failed(UserMutationFailureReason.Conflict);
    }

    public async Task<UserMutationResult> UpdateUserAsync(
        Guid userId,
        string email,
        string name,
        UserRole role,
        CancellationToken cancellationToken = default)
    {
        var validation = ValidateProfile(email, name, role);
        if (validation is not null)
        {
            return validation;
        }

        var user = await userRepository.FindByIdTrackedAsync(userId, cancellationToken);
        if (user is null)
        {
            return UserMutationResult.Failed(UserMutationFailureReason.NotFound);
        }

        var normalizedEmail = EmailRules.Normalize(email);
        if (!string.Equals(user.EmailNormalized, normalizedEmail, StringComparison.Ordinal))
        {
            var duplicate = await userRepository.FindByNormalizedEmailAsync(normalizedEmail, cancellationToken);
            if (duplicate is not null && duplicate.Id != userId)
            {
                return UserMutationResult.Failed(UserMutationFailureReason.DuplicateEmail);
            }
        }

        if (user.Role == UserRole.Admin &&
            role == UserRole.Employee &&
            user.IsActive &&
            await IsLastActiveAdminAsync(userId, cancellationToken))
        {
            return UserMutationResult.Failed(UserMutationFailureReason.LastActiveAdmin);
        }

        user.Email = email.Trim();
        user.EmailNormalized = normalizedEmail;
        user.Name = name.Trim();
        user.Role = role;
        user.UpdatedAt = DateTimeOffset.UtcNow;

        var saved = await userRepository.TrySaveChangesAsync(cancellationToken);
        return saved
            ? UserMutationResult.Success(user.Id)
            : UserMutationResult.Failed(UserMutationFailureReason.Conflict);
    }

    public async Task<UserMutationResult> DeactivateUserAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var user = await userRepository.FindByIdTrackedAsync(userId, cancellationToken);
        if (user is null)
        {
            return UserMutationResult.Failed(UserMutationFailureReason.NotFound);
        }

        if (!user.IsActive)
        {
            return UserMutationResult.Failed(UserMutationFailureReason.AlreadyInactive);
        }

        if (user.Role == UserRole.Admin &&
            await IsLastActiveAdminAsync(userId, cancellationToken))
        {
            return UserMutationResult.Failed(UserMutationFailureReason.LastActiveAdmin);
        }

        user.IsActive = false;
        user.UpdatedAt = DateTimeOffset.UtcNow;

        var saved = await userRepository.TrySaveChangesAsync(cancellationToken);
        return saved
            ? UserMutationResult.Success(user.Id)
            : UserMutationResult.Failed(UserMutationFailureReason.Conflict);
    }

    public async Task<ResetPasswordResult> ResetPasswordAsync(
        Guid userId,
        CancellationToken cancellationToken = default)
    {
        var user = await userRepository.FindByIdTrackedAsync(userId, cancellationToken);
        if (user is null)
        {
            return ResetPasswordResult.Failed(UserMutationFailureReason.NotFound);
        }

        var plaintext = PasswordRules.GenerateTemporaryPassword();
        user.PasswordHash = passwordVerifier.HashPassword(user, plaintext);
        user.UpdatedAt = DateTimeOffset.UtcNow;

        var saved = await userRepository.TrySaveChangesAsync(cancellationToken);
        return saved
            ? ResetPasswordResult.Success(plaintext)
            : ResetPasswordResult.Failed(UserMutationFailureReason.Conflict);
    }

    private static UserMutationResult? ValidateProfile(string email, string name, UserRole role)
    {
        if (string.IsNullOrWhiteSpace(email) || !email.Contains('@'))
        {
            return UserMutationResult.Failed(UserMutationFailureReason.InvalidEmail);
        }

        if (string.IsNullOrWhiteSpace(name))
        {
            return UserMutationResult.Failed(UserMutationFailureReason.InvalidName);
        }

        if (role is not (UserRole.Admin or UserRole.Employee))
        {
            return UserMutationResult.Failed(UserMutationFailureReason.InvalidRole);
        }

        return null;
    }

    private async Task<bool> IsLastActiveAdminAsync(
        Guid userId,
        CancellationToken cancellationToken)
    {
        var activeAdminCount = await userRepository.CountActiveAdminsAsync(
            excludeUserId: null,
            cancellationToken);

        if (activeAdminCount != 1)
        {
            return false;
        }

        var user = await userRepository.FindByIdAsync(userId, cancellationToken);
        return user is { Role: UserRole.Admin, IsActive: true };
    }
}
