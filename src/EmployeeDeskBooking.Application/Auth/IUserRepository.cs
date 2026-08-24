using EmployeeDeskBooking.Domain.Entities;

namespace EmployeeDeskBooking.Application.Auth;

public interface IUserRepository
{
    Task<User?> FindByEmailAsync(string email, CancellationToken cancellationToken = default);
}
