using EmployeeDeskBooking.Domain.Entities;
using EmployeeDeskBooking.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EmployeeDeskBooking.Infrastructure.Data.Configurations;

public class EmailDeliveryLogConfiguration : IEntityTypeConfiguration<EmailDeliveryLog>
{
    public void Configure(EntityTypeBuilder<EmailDeliveryLog> builder)
    {
        builder.ToTable("EmailDeliveryLogs");

        builder.HasKey(log => log.Id);

        builder.Property(log => log.EmailType)
            .HasConversion<byte>()
            .IsRequired();

        builder.Property(log => log.Recipient)
            .HasMaxLength(320)
            .IsRequired();

        builder.Property(log => log.Status)
            .HasConversion<byte>()
            .IsRequired();

        builder.Property(log => log.CreatedAt).IsRequired();

        builder.HasOne(log => log.Booking)
            .WithMany()
            .HasForeignKey(log => log.BookingId)
            .OnDelete(DeleteBehavior.SetNull);

        builder.HasOne(log => log.User)
            .WithMany()
            .HasForeignKey(log => log.UserId)
            .OnDelete(DeleteBehavior.SetNull);
    }
}

public class BookingReminderConfiguration : IEntityTypeConfiguration<BookingReminder>
{
    public void Configure(EntityTypeBuilder<BookingReminder> builder)
    {
        builder.ToTable("BookingReminders");

        builder.HasKey(reminder => reminder.BookingId);

        builder.Property(reminder => reminder.SentAt).IsRequired();
        builder.Property(reminder => reminder.CreatedAt).IsRequired();

        builder.HasOne(reminder => reminder.Booking)
            .WithMany()
            .HasForeignKey(reminder => reminder.BookingId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
