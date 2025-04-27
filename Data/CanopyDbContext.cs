using Canopy.API.Models.Events;
using Microsoft.EntityFrameworkCore;

namespace Canopy.API.Data;

public class CanopyDbContext : DbContext
{
    public CanopyDbContext(DbContextOptions<CanopyDbContext> options) : base(options)
    {
    }
    
    public DbSet<Event> Events { get; set; } = null!;
}