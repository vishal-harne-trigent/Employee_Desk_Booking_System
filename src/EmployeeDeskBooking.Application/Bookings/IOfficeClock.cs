namespace EmployeeDeskBooking.Application.Bookings;

public interface IOfficeClock
{
    TimeZoneInfo TimeZone { get; }

    DateOnly Today { get; }

    DateTimeOffset Now { get; }
}
