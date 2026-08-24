using EmployeeDeskBooking.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace EmployeeDeskBooking.Infrastructure.Data.Configurations;

public class NotificationPreferenceConfiguration : IEntityTypeConfiguration<NotificationPreference>
{
    public void Configure(EntityTypeBuilder<NotificationPreference> builder)
    {
        builder.ToTable("NotificationPreferences");

        builder.HasKey(preference => preference.UserId);

        builder.Property(preference => preference.PushOptIn)
            .HasDefaultValue(false)
            .IsRequired();

        builder.Property(preference => preference.UpdatedAt).IsRequired();

        builder.HasOne(preference => preference.User)
            .WithMany()
            .HasForeignKey(preference => preference.UserId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
