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
}