using Canopy.API.Models.Events;
using Microsoft.EntityFrameworkCore;

namespace Canopy.API.Data;

public class EventsRepository: IEventsRepository
{
    private readonly CanopyDbContext _context;

    public EventsRepository(CanopyDbContext context)
    {
        _context = context ?? throw new ArgumentNullException(nameof(context));
    }
    
    public async Task<Event> CreateAsync(Event @event) {
        ArgumentNullException.ThrowIfNull(@event);
        
        await _context.Events.AddAsync(@event);
        await _context.SaveChangesAsync();
        return @event;
    }

    public async Task<List<Event>> GetCurrentMonthEventsAsync(string userId)
    {
        ArgumentNullException.ThrowIfNull(userId);
        
        var now = DateTime.UtcNow;
        var monthStart = new DateTime(now.Year, now.Month, 1, 0, 0, 0, DateTimeKind.Utc);
        var nextMonthStart = monthStart.AddMonths(1);
        
        return await _context.Events.Where(e =>
                e.UserId == userId &&
                e.IsDeleted == null &&
                e.StartTime < nextMonthStart &&
                e.EndTime > monthStart
            )
            .ToListAsync();
    }
}