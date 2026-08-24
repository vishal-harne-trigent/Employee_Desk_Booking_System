using EmployeeDeskBooking.Application.Bookings;
using EmployeeDeskBooking.Application.Notifications;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace EmployeeDeskBooking.Infrastructure.HostedServices;

public sealed class ReminderEmailHostedService(
    IServiceScopeFactory scopeFactory,
    IOfficeClock officeClock,
    IConfiguration configuration,
    ILogger<ReminderEmailHostedService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var delay = CalculateDelayUntilNextRun();
            logger.LogInformation(
                "Next booking reminder run scheduled in {DelayMinutes} minutes.",
                delay.TotalMinutes);

            try
            {
                await Task.Delay(delay, stoppingToken);
            }
            catch (TaskCanceledException)
            {
                break;
            }

            if (stoppingToken.IsCancellationRequested)
            {
                break;
            }

            try
            {
                using var scope = scopeFactory.CreateScope();
                var notificationService = scope.ServiceProvider
                    .GetRequiredService<IBookingNotificationService>();

                await notificationService.SendDueRemindersAsync(stoppingToken);
                logger.LogInformation("Booking reminder job completed.");
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Booking reminder job failed.");
            }
        }
    }

    private TimeSpan CalculateDelayUntilNextRun()
    {
        var sendTimeText = configuration["Reminders:SendTimeLocal"] ?? "08:00";
        if (!TimeOnly.TryParse(sendTimeText, out var sendTime))
        {
            sendTime = new TimeOnly(8, 0);
        }

        var timeZone = officeClock.TimeZone;
        var localNow = TimeZoneInfo.ConvertTime(officeClock.Now, timeZone);
        var scheduledLocal = localNow.Date.Add(sendTime.ToTimeSpan());

        if (localNow >= scheduledLocal)
        {
            scheduledLocal = scheduledLocal.AddDays(1);
        }

        var scheduledUtc = TimeZoneInfo.ConvertTimeToUtc(scheduledLocal, timeZone);
        var delay = scheduledUtc - officeClock.Now;
        return delay <= TimeSpan.Zero ? TimeSpan.FromMinutes(1) : delay;
    }
}
