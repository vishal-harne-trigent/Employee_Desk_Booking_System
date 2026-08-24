namespace EmployeeDeskBooking.Domain.Entities;

public class BookingReminder
{
    public Guid BookingId { get; set; }

    public Booking Booking { get; set; } = null!;

    public DateTimeOffset SentAt { get; set; }

    public DateTimeOffset CreatedAt { get; set; }
}
