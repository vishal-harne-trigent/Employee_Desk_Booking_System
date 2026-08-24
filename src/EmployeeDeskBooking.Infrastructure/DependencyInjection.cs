using EmployeeDeskBooking.Application.Auth;
using EmployeeDeskBooking.Infrastructure.Data;
using EmployeeDeskBooking.Infrastructure.Repositories;
using EmployeeDeskBooking.Infrastructure.Security;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace EmployeeDeskBooking.Infrastructure;

public static class DependencyInjection
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddSingleton<IPasswordVerifier, PasswordVerifier>();

        return services;
    }
}
