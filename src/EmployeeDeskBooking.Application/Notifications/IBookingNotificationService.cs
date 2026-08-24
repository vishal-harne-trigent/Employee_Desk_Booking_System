namespace EmployeeDeskBooking.Application.Notifications;

public interface IBookingNotificationService
{
    Task SendConfirmationAsync(Guid bookingId, CancellationToken cancellationToken = default);

    Task SendCancellationAsync(Guid bookingId, CancellationToken cancellationToken = default);

    Task SendDueRemindersAsync(CancellationToken cancellationToken = default);
}
