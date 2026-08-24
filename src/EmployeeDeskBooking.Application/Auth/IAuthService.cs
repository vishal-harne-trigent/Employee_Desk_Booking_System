namespace EmployeeDeskBooking.Application.Auth;

public interface IAuthService
{
    Task<SignInResult> SignInAsync(string email, string password, CancellationToken cancellationToken = default);
}
