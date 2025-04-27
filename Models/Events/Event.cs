using System.ComponentModel.DataAnnotations;

namespace Canopy.API.Models.Events;

public class Event
{
    [Key]
    public int Id { get; init; }
    
    [Required]
    [MaxLength(100)]
    public required string UserId { get; init; }
    
    [Required]
    [MaxLength(100)]
    public required string Name { get; init; }
    
    [Required]
    [MaxLength(7)]
    public required string Color { get; init; }
    
    [Required]
    public required DateTimeOffset StartTime { get; init; }
    
    [Required]
    public required DateTimeOffset EndTime { get; init; }
    
    [MaxLength(100)]
    public string? Location { get; init; }
    
    [MaxLength(500)]
    public string? Description { get; init; }
    
    [MaxLength(100)]
    public string? RecurrenceRule { get; init; }
    
    public bool? IsDeleted { get; init; }
}