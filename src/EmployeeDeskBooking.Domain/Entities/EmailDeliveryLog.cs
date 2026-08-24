using EmployeeDeskBooking.Domain.Enums;

namespace EmployeeDeskBooking.Domain.Entities;

public class EmailDeliveryLog
{
    public Guid Id { get; set; }

    public Guid? BookingId { get; set; }

    public Booking? Booking { get; set; }

    public Guid? UserId { get; set; }

    public User? User { get; set; }

    public BookingEmailType EmailType { get; set; }

    public string Recipient { get; set; } = string.Empty;

    public EmailDeliveryStatus Status { get; set; }

    public string? ErrorMessage { get; set; }

    public DateTimeOffset CreatedAt { get; set; }
}
