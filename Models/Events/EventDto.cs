namespace Canopy.API.Models.Events;

public record EventDto()
{
    public int Id { get; init; }
    public required string UserId { get; init; }
    public required string Name { get; init; }
    public required string Color { get; init; }
    public required DateTime StartTime { get; init; }
    public required DateTime EndTime { get; init; }
    public string? RecurrenceRule { get; init; }
    public string? Location { get; init; }
    public string? Description { get; init; }
    public bool? IsDeleted { get; init; }
}