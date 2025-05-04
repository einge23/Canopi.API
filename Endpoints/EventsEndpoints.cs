using System.Security.Claims;
using Canopy.API.Filters;
using Canopy.API.Models.Events;
using Canopy.API.Services;
using Microsoft.AspNetCore.Mvc;

namespace Canopy.API.Endpoints;

public class EventEndpointsLogger { }

public static class EventsEndpoints
{
    public static RouteGroupBuilder MapEventsEndpoints(this RouteGroupBuilder group)
    {
        //Route: POST /api/events/create
        group.MapPost("/create", CreateEvent)
            .WithName("CreateEvent")
            .Accepts<CreateEventRequest>("application/json")
            .Produces<EventDto>(StatusCodes.Status201Created)
            .ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .AddEndpointFilter<ValidationFilter<CreateEventRequest>>();
        
        //Route: GET /api/events/
        group.MapGet("/", GetMonthlyEvents)
            .WithName("GetMonthlyEvents")
            .Produces<List<EventDto>>(StatusCodes.Status200OK)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .ProducesProblem(StatusCodes.Status401Unauthorized);

        //Route: PUT /api/events/edit
        group.MapPut("/edit", UpdateEvent)
            .WithName("UpdateEvent")
            .Accepts<EventDto>("application/json")
            .Produces<EventDto>(StatusCodes.Status200OK)
            .ProducesValidationProblem(StatusCodes.Status422UnprocessableEntity)
            .ProducesProblem(StatusCodes.Status500InternalServerError)
            .ProducesProblem(StatusCodes.Status401Unauthorized)
            .AddEndpointFilter<ValidationFilter<EventDto>>();

        
        return group;
    }

    private static async Task<IResult> CreateEvent(
        [FromBody] CreateEventRequest request,
        IEventsService eventsService,
        HttpContext httpContext,
        LinkGenerator linkGenerator,
        ILogger<EventEndpointsLogger> logger
        )
    {
        
        try
        {
            var userId = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)
                         ?? httpContext.User.FindFirstValue("sub");

            logger.LogInformation("Attempting to use UserId: {UserId}", userId ?? "null");

            if (string.IsNullOrEmpty(userId))
            {
                logger.LogWarning("User ID claim ('sub' or NameIdentifier) not found after successful authentication.");
                return Results.Unauthorized();
            }
            
            var safeRequest = request with { UserId = userId };
            var createdEvent = await eventsService.CreateEvent(safeRequest);
            var locationUrl = linkGenerator.GetUriByName(
                httpContext,
                "GetEventById",
                new { id = createdEvent.Id }
            );
            
            return Results.Created(locationUrl, createdEvent);
        }
        catch (ArgumentException ex)
        {
            return Results.ValidationProblem(new Dictionary<string, string[]>
            {
                { "Error", new[] { ex.Message } }
            });
        }
        catch (Exception ex)
        {
            return Results.Problem(ex.Message, statusCode: StatusCodes.Status500InternalServerError);
        }
    }

    private static async Task<IResult> GetMonthlyEvents(
        [FromQuery] int month,
        [FromQuery] int year,
        [FromQuery] string timezone,
        IEventsService eventsService,
        HttpContext httpContext,
        LinkGenerator linkGenerator,
        ILogger<EventEndpointsLogger> logger)
    {
        var validationErrors = new Dictionary<string, string[]>();
            
        if (year <= 0)
        {
            validationErrors.Add(nameof(year), new[] { "Year must be a positive integer." });
        }

        if (month < 1 || month > 12)
        {
            validationErrors.Add(nameof(month), new[] { "Month must be between 1 and 12." });
        }

        if (string.IsNullOrEmpty(timezone))
        {
            validationErrors.Add(nameof(timezone), new[] { "Timezone is required." });
        }

        if (validationErrors.Count > 0)
        {
            return Results.ValidationProblem(validationErrors, statusCode: StatusCodes.Status422UnprocessableEntity);
        }
        
        
        var userId = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)
                     ?? httpContext.User.FindFirstValue("sub");
        
        if (string.IsNullOrEmpty(userId))
        {
            logger.LogWarning("User ID claim ('sub' or NameIdentifier) not found after successful authentication.");
            return Results.Unauthorized();
        }

        try
        {
            var events = await eventsService.GetMonthlyEvents(userId, year, month, timezone);
           
            return Results.Ok(events);
        }
        catch (Exception ex)
        {
            return Results.Problem(ex.Message, statusCode: StatusCodes.Status500InternalServerError);
        }
    }

    private static async Task<IResult> UpdateEvent(
        [FromBody] EventDto request,
        IEventsService eventsService,
        HttpContext httpContext,
        LinkGenerator linkGenerator,
        ILogger<EventEndpointsLogger> logger
    )
    {
        var userId = httpContext.User.FindFirstValue(ClaimTypes.NameIdentifier)
                     ?? httpContext.User.FindFirstValue("sub");
        
        if (string.IsNullOrEmpty(userId) || request.UserId != userId)
        {
            logger.LogWarning("User ID claim ('sub' or NameIdentifier) not found after successful authentication.");
            return Results.Unauthorized();
        }

        try
        {
            var updatedEvent = await eventsService.UpdateEvent(request);
            return Results.Ok(updatedEvent);
        }
        catch (ArgumentException ex)
        {
            return Results.ValidationProblem(new Dictionary<string, string[]>
            {
                { "Error", new[] { ex.Message } }
            });
        }
        catch (Exception ex)
        {
            return Results.Problem(ex.Message, statusCode: StatusCodes.Status500InternalServerError);
        }
    }
}