namespace EmployeeDeskBooking.Domain.Entities;

public class NotificationPreference
{
    public Guid UserId { get; set; }

    public User User { get; set; } = null!;

    public bool PushOptIn { get; set; }

    public string? PushSubscription { get; set; }

    public DateTimeOffset UpdatedAt { get; set; }
}
