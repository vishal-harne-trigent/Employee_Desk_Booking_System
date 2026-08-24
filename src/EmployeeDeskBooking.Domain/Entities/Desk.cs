using EmployeeDeskBooking.Domain.Enums;

namespace EmployeeDeskBooking.Domain.Entities;

public class Desk
{
    public Guid Id { get; set; }

    public string DeskNumber { get; set; } = string.Empty;

    public string DeskNumberNormalized { get; set; } = string.Empty;

    public DeskStatus Status { get; set; }

    public DateTimeOffset CreatedAt { get; set; }

    public DateTimeOffset UpdatedAt { get; set; }
}
