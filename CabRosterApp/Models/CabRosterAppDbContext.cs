using CabRosterApp.Models;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

public class CabRosterAppDbContext : IdentityDbContext<ApplicationUser>
{
    private readonly ILogger<CabRosterAppDbContext> _logger;

    public CabRosterAppDbContext(DbContextOptions<CabRosterAppDbContext> options, ILogger<CabRosterAppDbContext> logger)
        : base(options)
    {
        _logger = logger;
    }

    public DbSet<Shift> Shifts { get; set; }
    public DbSet<CabBooking> CabBookings { get; set; }
    public DbSet<NodalPoint> NodalPoints { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // Seed shifts with logging
        modelBuilder.Entity<Shift>().HasData(
            new Shift { Id = 1, ShiftTime = "10AM to 7PM" },
            new Shift { Id = 2, ShiftTime = "12PM to 9PM" },
            new Shift { Id = 3, ShiftTime = "4PM to 1AM" },
            new Shift { Id = 4, ShiftTime = "6PM to 3AM" }
        );

        _logger.LogInformation("Seed data for shifts completed.");

        // Define relationships
        modelBuilder.Entity<CabBooking>()
            .HasOne(cb => cb.User)
            .WithMany()
            .HasForeignKey(cb => cb.UserId);

        modelBuilder.Entity<CabBooking>()
            .HasOne(cb => cb.Shift)
            .WithMany()
            .HasForeignKey(cb => cb.ShiftId);

        modelBuilder.Entity<CabBooking>()
            .HasOne(cb => cb.NodalPoint) // Added relationship to NodalPoint
            .WithMany()
            .HasForeignKey(cb => cb.NodalPointId);
    }
}
