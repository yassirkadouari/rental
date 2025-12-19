using CarRentalApp.Core.Entities;
using Microsoft.EntityFrameworkCore;

namespace CarRentalApp.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options)
    {
    }

    public DbSet<User> Users { get; set; }
    public DbSet<Vehicle> Vehicles { get; set; }
    public DbSet<VehicleType> VehicleTypes { get; set; }
    public DbSet<Booking> Bookings { get; set; }
    public DbSet<Maintenance> Maintenances { get; set; }
    public DbSet<Payment> Payments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);
        
        // Configure relationships and constraints here if needed
        modelBuilder.Entity<Vehicle>()
            .Property(v => v.DailyRate)
            .HasColumnType("decimal(18,2)");
            
        modelBuilder.Entity<Booking>()
            .Property(b => b.TotalPrice)
            .HasColumnType("decimal(18,2)");
    }
}
