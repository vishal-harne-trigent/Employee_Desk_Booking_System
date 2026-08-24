using EmployeeDeskBooking.Application.Bookings;
using Microsoft.Extensions.Configuration;

namespace EmployeeDeskBooking.Infrastructure.Time;

public sealed class OfficeClock(IConfiguration configuration) : IOfficeClock
{
    private readonly TimeZoneInfo _timeZone = ResolveTimeZone(configuration);

    public TimeZoneInfo TimeZone => _timeZone;

    public DateOnly Today => DateOnly.FromDateTime(TimeZoneInfo.ConvertTime(Now, _timeZone).DateTime);

    public DateTimeOffset Now => DateTimeOffset.UtcNow;

    private static TimeZoneInfo ResolveTimeZone(IConfiguration configuration)
    {
        var timeZoneId = configuration["Office:TimeZone"] ?? "Asia/Kolkata";

        try
        {
            return TimeZoneInfo.FindSystemTimeZoneById(timeZoneId);
        }
        catch (TimeZoneNotFoundException)
        {
            return TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
        }
    }
}
