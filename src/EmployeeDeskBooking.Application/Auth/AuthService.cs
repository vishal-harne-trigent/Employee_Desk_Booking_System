using EmployeeDeskBooking.Domain.Enums;

namespace EmployeeDeskBooking.Application.Auth;

public sealed class AuthService(IUserRepository userRepository, IPasswordVerifier passwordVerifier)
    : IAuthService
{
    public async Task<SignInResult> SignInAsync(
        string email,
        string password,
        CancellationToken cancellationToken = default)
    {
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            return SignInResult.Failed(SignInFailureReason.InvalidCredentials);
        }

        var user = await userRepository.FindByEmailAsync(email, cancellationToken);
        if (user is null || !passwordVerifier.VerifyPassword(user, password))
        {
            return SignInResult.Failed(SignInFailureReason.InvalidCredentials);
        }

        if (!user.IsActive)
        {
            return SignInResult.Failed(SignInFailureReason.DeactivatedAccount);
        }

        return SignInResult.Success(new SignInUser(
            user.Id,
            user.Email,
            user.Name,
            user.Role.ToString()));
    }
}
