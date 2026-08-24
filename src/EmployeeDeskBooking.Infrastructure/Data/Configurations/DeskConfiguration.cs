using EmployeeDeskBooking.Domain.Entities;
using EmployeeDeskBooking.Domain.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EmployeeDeskBooking.Infrastructure.Data.Configurations;

public class DeskConfiguration : IEntityTypeConfiguration<Desk>
{
    public void Configure(EntityTypeBuilder<Desk> builder)
    {
        builder.ToTable("Desks");

        builder.HasKey(desk => desk.Id);

        builder.Property(desk => desk.DeskNumber)
            .HasMaxLength(32)
            .IsRequired();

        builder.Property(desk => desk.DeskNumberNormalized)
            .HasMaxLength(32)
            .IsRequired();

        builder.HasIndex(desk => desk.DeskNumberNormalized)
            .IsUnique();

        builder.Property(desk => desk.Status)
            .HasConversion<byte>()
            .IsRequired();

        builder.Property(desk => desk.CreatedAt).IsRequired();
        builder.Property(desk => desk.UpdatedAt).IsRequired();
    }
}

public class BookingConfiguration : IEntityTypeConfiguration<Booking>
{
    public void Configure(EntityTypeBuilder<Booking> builder)
    {
        builder.ToTable("Bookings");

        builder.HasKey(booking => booking.Id);

        builder.Property(booking => booking.BookingDate)
            .HasColumnType("date")
            .IsRequired();

        builder.Property(booking => booking.Status)
            .HasConversion<byte>()
            .IsRequired();

        builder.HasOne(booking => booking.User)
            .WithMany()
            .HasForeignKey(booking => booking.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasOne(booking => booking.Desk)
            .WithMany()
            .HasForeignKey(booking => booking.DeskId)
            .OnDelete(DeleteBehavior.Restrict);

        builder.HasIndex(booking => new { booking.UserId, booking.BookingDate })
            .IsUnique()
            .HasFilter("[Status] = 0");

        builder.HasIndex(booking => new { booking.DeskId, booking.BookingDate })
            .IsUnique()
            .HasFilter("[Status] = 0");

        builder.HasIndex(booking => new { booking.BookingDate, booking.Status });
        builder.HasIndex(booking => new { booking.UserId, booking.BookingDate });

        builder.Property(booking => booking.CreatedAt).IsRequired();
        builder.Property(booking => booking.UpdatedAt).IsRequired();
    }
}
