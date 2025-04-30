using AutoMapper;
using Canopy.API.Data;
using Canopy.API.Models.Events;

namespace Canopy.API.Services;

public class EventsService : IEventsService
{
    private readonly IEventsRepository _eventsRepository;
    private readonly IMapper _mapper;
    private readonly ILogger<EventsService> _logger;

    public EventsService(IEventsRepository eventsRepository, IMapper mapper, ILogger<EventsService> logger)
    {
        _mapper = mapper;
        _eventsRepository = eventsRepository;
        _logger = logger;
    }

    public async Task<EventDto> CreateEvent(CreateEventRequest request)
    {
        if (request.EndTime <= request.StartTime)
        {
            throw new ArgumentException("Event end time must be after the start time.");
        }

        _logger.LogInformation("Creating new event");
        var eventEntity = _mapper.Map<Event>(request);
        _logger.LogDebug("Event entity mapped from request: {@EventEntity}", eventEntity);
        
        var createdEvent = await _eventsRepository.CreateAsync(eventEntity);
        return _mapper.Map<EventDto>(createdEvent);
    }

    public async Task<List<EventDto>> GetCurrentMonthEvents(string userId)
    {
        var events = await _eventsRepository.GetCurrentMonthEventsAsync(userId);
        
        if (events.Count == 0)
        {
            _logger.LogInformation("No events found for user {UserId} in the current month.", userId);
            return new List<EventDto>();
        }
        
        _logger.LogInformation("Found {EventCount} events for user {UserId} in the current month.", events.Count, userId);
        var eventDtos = _mapper.Map<List<EventDto>>(events);
        
        return eventDtos;
    }

    public async Task<EventDto> UpdateEvent(EventDto request)
    {
        if (request.EndTime <= request.StartTime)
        {
            throw new ArgumentException("Event end time must be after the start time.");
        }

        _logger.LogInformation("Updating event with ID {EventId}", request.Id);
        var eventEntity = _mapper.Map<Event>(request);
        _logger.LogDebug("Event entity mapped from request: {@EventEntity}", eventEntity);
        
        var updatedEvent = await _eventsRepository.UpdateAsync(eventEntity);
        return _mapper.Map<EventDto>(updatedEvent);
    }
}