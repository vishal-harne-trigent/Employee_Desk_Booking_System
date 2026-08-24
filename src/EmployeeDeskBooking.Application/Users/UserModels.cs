using EmployeeDeskBooking.Domain.Enums;

namespace EmployeeDeskBooking.Application.Users;

public enum UserMutationFailureReason
{
    None = 0,
    InvalidEmail = 1,
    InvalidName = 2,
    InvalidPassword = 3,
    InvalidRole = 4,
    DuplicateEmail = 5,
    NotFound = 6,
    LastActiveAdmin = 7,
    AlreadyInactive = 8,
    Conflict = 9,
}

public sealed record UserListItem(
    Guid UserId,
    string Name,
    string Email,
    UserRole Role,
    bool IsActive,
    bool IsLastActiveAdmin);

public sealed record UserMutationResult(
    bool Succeeded,
    Guid? UserId,
    UserMutationFailureReason FailureReason)
{
    public static UserMutationResult Success(Guid userId) =>
        new(true, userId, UserMutationFailureReason.None);

    public static UserMutationResult Failed(UserMutationFailureReason reason) =>
        new(false, null, reason);
}

public sealed record ResetPasswordResult(
    bool Succeeded,
    string? PlaintextPassword,
    UserMutationFailureReason FailureReason)
{
    public static ResetPasswordResult Success(string plaintextPassword) =>
        new(true, plaintextPassword, UserMutationFailureReason.None);

    public static ResetPasswordResult Failed(UserMutationFailureReason reason) =>
        new(false, null, reason);
}
