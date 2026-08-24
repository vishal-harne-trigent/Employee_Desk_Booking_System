namespace EmployeeDeskBooking.Api.Models;

public sealed class CreateDeskRequest
{
    public string DeskNumber { get; set; } = string.Empty;
}

public sealed class UpdateDeskRequest
{
    public string? DeskNumber { get; set; }

    public string? Status { get; set; }
}

public sealed class AdminDeskResponse
{
    public Guid DeskId { get; set; }

    public string DeskNumber { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;

    public bool CanDeactivate { get; set; }

    public int BlockingBookingCount { get; set; }
}
