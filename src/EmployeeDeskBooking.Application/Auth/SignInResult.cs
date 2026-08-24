namespace EmployeeDeskBooking.Application.Auth;

public enum SignInFailureReason
{
    None = 0,
    InvalidCredentials = 1,
    DeactivatedAccount = 2,
}

public sealed record SignInResult(bool Succeeded, SignInUser? User, SignInFailureReason FailureReason)
{
    public static SignInResult Success(SignInUser user) =>
        new(true, user, SignInFailureReason.None);

    public static SignInResult Failed(SignInFailureReason reason) =>
        new(false, null, reason);
}

public sealed record SignInUser(
    Guid Id,
    string Email,
    string Name,
    string Role);
