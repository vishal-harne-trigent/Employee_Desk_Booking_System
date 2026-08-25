using EmployeeDeskBooking.Application.Bookings;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace EmployeeDeskBooking.Infrastructure.HostedServices;

public sealed class CompletePastBookingsHostedService(
    IServiceScopeFactory scopeFactory,
    IOfficeClock officeClock,
    IConfiguration configuration,
    ILogger<CompletePastBookingsHostedService> logger) : BackgroundService
{
    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            var delay = CalculateDelayUntilNextRun();
            logger.LogInformation(
                "Next booking completion run scheduled in {DelayMinutes} minutes.",
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
                var bookingService = scope.ServiceProvider.GetRequiredService<IBookingService>();
                var completedCount = await bookingService.CompletePastBookingsAsync(stoppingToken);
                logger.LogInformation(
                    "Booking completion job finished. {CompletedCount} booking(s) marked Completed.",
                    completedCount);
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "Booking completion job failed.");
            }
        }
    }

    private TimeSpan CalculateDelayUntilNextRun()
    {
        var runTimeText = configuration["Completion:RunTimeLocal"] ?? "00:05";
        if (!TimeOnly.TryParse(runTimeText, out var runTime))
        {
            runTime = new TimeOnly(0, 5);
        }

        var timeZone = officeClock.TimeZone;
        var localNow = TimeZoneInfo.ConvertTime(officeClock.Now, timeZone);
        var scheduledLocal = localNow.Date.Add(runTime.ToTimeSpan());

        if (localNow >= scheduledLocal)
        {
            scheduledLocal = scheduledLocal.AddDays(1);
        }

        var scheduledUtc = TimeZoneInfo.ConvertTimeToUtc(scheduledLocal, timeZone);
        var delay = scheduledUtc - officeClock.Now;
        return delay <= TimeSpan.Zero ? TimeSpan.FromMinutes(1) : delay;
    }
}
