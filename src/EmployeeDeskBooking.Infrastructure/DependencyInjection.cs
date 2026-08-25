using EmployeeDeskBooking.Application.Auth;
using EmployeeDeskBooking.Application.Bookings;
using EmployeeDeskBooking.Application.Desks;
using EmployeeDeskBooking.Application.Notifications;
using EmployeeDeskBooking.Infrastructure.Data;
using EmployeeDeskBooking.Infrastructure.Email;
using EmployeeDeskBooking.Infrastructure.HostedServices;
using EmployeeDeskBooking.Infrastructure.Push;
using EmployeeDeskBooking.Infrastructure.Repositories;
using EmployeeDeskBooking.Infrastructure.Security;
using EmployeeDeskBooking.Infrastructure.Time;
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
        services.AddSingleton<IOfficeClock, OfficeClock>();
        services.AddScoped<IDeskRepository, DeskRepository>();
        services.AddScoped<IBookingRepository, BookingRepository>();
        services.AddScoped<IEmailDeliveryRepository, EmailDeliveryRepository>();
        services.AddScoped<INotificationPreferenceRepository, NotificationPreferenceRepository>();

        if (string.IsNullOrWhiteSpace(configuration["Smtp:Host"]))
        {
            services.AddSingleton<IEmailSender, FileEmailSender>();
        }
        else
        {
            services.AddSingleton<IEmailSender, SmtpEmailSender>();
        }

        if (string.IsNullOrWhiteSpace(configuration["Vapid:PublicKey"]))
        {
            services.AddSingleton<IPushNotificationSender, FilePushNotificationSender>();
        }
        else
        {
            services.AddSingleton<IPushNotificationSender, WebPushNotificationSender>();
        }

        return services;
    }

    public static IServiceCollection AddBookingReminderJob(this IServiceCollection services)
    {
        services.AddHostedService<ReminderEmailHostedService>();
        return services;
    }

    public static IServiceCollection AddBookingCompletionJob(this IServiceCollection services)
    {
        services.AddHostedService<CompletePastBookingsHostedService>();
        return services;
    }
}
