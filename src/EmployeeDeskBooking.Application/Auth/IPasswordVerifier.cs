using EmployeeDeskBooking.Domain.Entities;

namespace EmployeeDeskBooking.Application.Auth;

public interface IPasswordVerifier
{
    string HashPassword(User user, string password);

    bool VerifyPassword(User user, string password);
}
