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

    public async Task<List<Event>> GetCurrentMonthEventsAsync(string userId, string userTimeZoneId)
    {
        ArgumentNullException.ThrowIfNull(userId);

        
        TimeZoneInfo userTimeZone;
        try
        {
            userTimeZone = TimeZoneInfo.FindSystemTimeZoneById(userTimeZoneId);
        }
        catch (TimeZoneNotFoundException)
        {
            userTimeZone = TimeZoneInfo.Utc;
        }
        var nowInUserTimeZone = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, userTimeZone);

        var monthStartInUserTimeZone = new DateTime(nowInUserTimeZone.Year, nowInUserTimeZone.Month, 1);

        var nextMonthStartInUserTimeZone = monthStartInUserTimeZone.AddMonths(1);

        var monthStartUtc = TimeZoneInfo.ConvertTimeToUtc(monthStartInUserTimeZone, userTimeZone);
        var nextMonthStartUtc = TimeZoneInfo.ConvertTimeToUtc(nextMonthStartInUserTimeZone, userTimeZone);

        return await _context.Events.Where(e =>
                e.UserId == userId &&
                e.IsDeleted == null &&
                e.StartTime < nextMonthStartUtc &&
                e.EndTime > monthStartUtc
            )
            .ToListAsync();
    }
    
    public async Task<Event> UpdateAsync(Event @event)
    {
        ArgumentNullException.ThrowIfNull(@event);
        
        var existingEvent = await _context.Events.FindAsync(@event.Id);
        if (existingEvent == null)
        {
            throw new KeyNotFoundException($"Event with ID {@event.Id} not found.");
        }
        _context.Entry(existingEvent).CurrentValues.SetValues(@event);
        await _context.SaveChangesAsync();
        return @event;
    }
}