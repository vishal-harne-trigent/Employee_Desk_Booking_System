using EmployeeDeskBooking.Domain.Entities;
using EmployeeDeskBooking.Infrastructure.Data.Configurations;
using Microsoft.EntityFrameworkCore;

namespace EmployeeDeskBooking.Infrastructure.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();

    public DbSet<Desk> Desks => Set<Desk>();

    public DbSet<Booking> Bookings => Set<Booking>();

    public DbSet<EmailDeliveryLog> EmailDeliveryLogs => Set<EmailDeliveryLog>();

    public DbSet<BookingReminder> BookingReminders => Set<BookingReminder>();

    public DbSet<NotificationPreference> NotificationPreferences => Set<NotificationPreference>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        modelBuilder.ApplyConfiguration(new DeskConfiguration());
        modelBuilder.ApplyConfiguration(new BookingConfiguration());
        modelBuilder.ApplyConfiguration(new EmailDeliveryLogConfiguration());
        modelBuilder.ApplyConfiguration(new BookingReminderConfiguration());
        modelBuilder.ApplyConfiguration(new NotificationPreferenceConfiguration());
        base.OnModelCreating(modelBuilder);
    }
}
