using Canopy.API.Models.Events;

namespace Canopy.API.Data;

public interface IEventsRepository
{
    Task<Event> CreateAsync(Event @event);
    Task<List<Event>> GetCurrentMonthEventsAsync(string userId);
    Task<Event> UpdateAsync(Event @event);
}