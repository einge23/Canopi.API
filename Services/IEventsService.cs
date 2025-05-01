using Canopy.API.Models.Events;

namespace Canopy.API.Services;

public interface IEventsService
{
    Task<EventDto> CreateEvent(CreateEventRequest request);
    Task<List<EventDto>> GetCurrentMonthEvents(string userId, string userTimeZoneId);
    Task<EventDto> UpdateEvent(EventDto request);
}