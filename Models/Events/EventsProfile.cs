using AutoMapper;

namespace Canopy.API.Models.Events;

public class EventsProfile : Profile
{
    public EventsProfile()
    {
        CreateMap<EventDto, Event>();
        CreateMap<Event, EventDto>();
        CreateMap<CreateEventRequest, Event>();
    }
}