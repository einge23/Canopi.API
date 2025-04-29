using Canopy.API.Models.Events;
using Microsoft.EntityFrameworkCore;

namespace Canopy.API.Data;

public class CanopyDbContext : DbContext
{
    public CanopyDbContext(DbContextOptions<CanopyDbContext> options) : base(options)
    {
    }
    
    public DbSet<Event> Events { get; set; } = null!;
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Event>()
            .Property(e => e.StartTime)
            .HasColumnType("timestamp with time zone");
        modelBuilder.Entity<Event>()
            .Property(e => e.EndTime)
            .HasColumnType("timestamp with time zone");

        base.OnModelCreating(modelBuilder);

    }
}

