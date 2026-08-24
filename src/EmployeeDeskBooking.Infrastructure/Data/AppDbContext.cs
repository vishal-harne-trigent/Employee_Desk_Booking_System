using EmployeeDeskBooking.Domain.Entities;
using EmployeeDeskBooking.Infrastructure.Data.Configurations;
using Microsoft.EntityFrameworkCore;

namespace EmployeeDeskBooking.Infrastructure.Data;

public class AppDbContext(DbContextOptions<AppDbContext> options) : DbContext(options)
{
    public DbSet<User> Users => Set<User>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.ApplyConfiguration(new UserConfiguration());
        base.OnModelCreating(modelBuilder);
    }
}
