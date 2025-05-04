namespace Canopy.API.Models.Events;

public record GetMonthlyEventsQuery
{
    public required int Year { get; init; }
    public required int Month { get; init; }
    public required string Timezone { get; init; }
}