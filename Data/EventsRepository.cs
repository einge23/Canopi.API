using Canopy.API.Models.Events;

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
}