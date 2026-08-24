using EmployeeDeskBooking.Domain.Enums;

namespace EmployeeDeskBooking.Application.Notifications;

public interface IEmailDeliveryRepository
{
    Task AddLogAsync(
        Guid? bookingId,
        Guid? userId,
        BookingEmailType emailType,
        string recipient,
        EmailDeliveryStatus status,
        string? errorMessage,
        DateTimeOffset createdAt,
        CancellationToken cancellationToken = default);

    Task<bool> HasReminderSentAsync(
        Guid bookingId,
        CancellationToken cancellationToken = default);

    Task AddReminderAsync(
        Guid bookingId,
        DateTimeOffset sentAt,
        CancellationToken cancellationToken = default);

    Task<bool> TrySaveChangesAsync(CancellationToken cancellationToken = default);
}
