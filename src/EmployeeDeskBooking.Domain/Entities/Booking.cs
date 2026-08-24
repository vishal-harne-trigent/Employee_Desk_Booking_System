using EmployeeDeskBooking.Domain.Enums;

namespace EmployeeDeskBooking.Domain.Entities;

public class Booking
{
    public Guid Id { get; set; }

    public Guid UserId { get; set; }

    public User User { get; set; } = null!;

    public Guid DeskId { get; set; }

    public Desk Desk { get; set; } = null!;

    public DateOnly BookingDate { get; set; }

    public BookingStatus Status { get; set; }

    public DateTimeOffset? CancelledAt { get; set; }

    public Guid? CancelledById { get; set; }

    public DateTimeOffset? CompletedAt { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset UpdatedAt { get; set; }
}
