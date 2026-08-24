namespace EmployeeDeskBooking.Api.Models;

public sealed class CreateBookingRequest
{
    public Guid DeskId { get; set; }

    public DateOnly BookingDate { get; set; }
}

public sealed class CreateBookingResponse
{
    public Guid BookingId { get; set; }

    public string DeskNumber { get; set; } = string.Empty;

    public DateOnly BookingDate { get; set; }
}

public sealed class AvailabilityResponse
{
    public DateOnly Date { get; set; }

    public IReadOnlyList<DeskAvailabilityResponse> Desks { get; set; } =
        Array.Empty<DeskAvailabilityResponse>();

    public ExistingBookingResponse? ExistingBooking { get; set; }
}

public sealed class DeskAvailabilityResponse
{
    public Guid DeskId { get; set; }

    public string DeskNumber { get; set; } = string.Empty;

    public bool IsAvailable { get; set; }
}

public sealed class ExistingBookingResponse
{
    public Guid BookingId { get; set; }

    public string DeskNumber { get; set; } = string.Empty;
}

public sealed class MyBookingResponse
{
    public Guid BookingId { get; set; }

    public DateOnly BookingDate { get; set; }

    public string DeskNumber { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;

    public bool CanCancel { get; set; }
}

public sealed class AdminBookingResponse
{
    public Guid BookingId { get; set; }

    public DateOnly BookingDate { get; set; }

    public string DeskNumber { get; set; } = string.Empty;

    public string EmployeeEmail { get; set; } = string.Empty;

    public string Status { get; set; } = string.Empty;

    public bool CanCancel { get; set; }
}
