using Canopy.API.Models.Events;

namespace Canopy.API.Data;

public interface IEventsRepository
{
    Task<Event> CreateAsync(Event @event);
    Task<List<Event>> GetMonthlyEventsAsync(string userId, int year, int month, string userTimeZoneId);
    Task<Event> UpdateAsync(Event @event);
}