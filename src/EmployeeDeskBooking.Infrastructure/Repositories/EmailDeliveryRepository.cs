using EmployeeDeskBooking.Application.Notifications;
using EmployeeDeskBooking.Domain.Entities;
using EmployeeDeskBooking.Domain.Enums;
using EmployeeDeskBooking.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace EmployeeDeskBooking.Infrastructure.Repositories;

public sealed class EmailDeliveryRepository(AppDbContext dbContext) : IEmailDeliveryRepository
{
    public Task AddLogAsync(
        Guid? bookingId,
        Guid? userId,
        BookingEmailType emailType,
        string recipient,
        EmailDeliveryStatus status,
        string? errorMessage,
        DateTimeOffset createdAt,
        CancellationToken cancellationToken = default)
    {
        dbContext.EmailDeliveryLogs.Add(new EmailDeliveryLog
        {
            Id = Guid.NewGuid(),
            BookingId = bookingId,
            UserId = userId,
            EmailType = emailType,
            Recipient = recipient,
            Status = status,
            ErrorMessage = errorMessage,
            CreatedAt = createdAt,
        });

        return Task.CompletedTask;
    }

    public async Task<bool> HasReminderSentAsync(
        Guid bookingId,
        CancellationToken cancellationToken = default) =>
        await dbContext.BookingReminders
            .AsNoTracking()
            .AnyAsync(reminder => reminder.BookingId == bookingId, cancellationToken);

    public Task AddReminderAsync(
        Guid bookingId,
        DateTimeOffset sentAt,
        CancellationToken cancellationToken = default)
    {
        dbContext.BookingReminders.Add(new BookingReminder
        {
            BookingId = bookingId,
            SentAt = sentAt,
            CreatedAt = sentAt,
        });

        return Task.CompletedTask;
    }

    public async Task<bool> TrySaveChangesAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await dbContext.SaveChangesAsync(cancellationToken);
            return true;
        }
        catch (DbUpdateException)
        {
            return false;
        }
    }
}
