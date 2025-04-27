using Canopy.API.Models.Events;
using FluentValidation;

namespace Canopy.API.Validators.Events;

public class CreateEventRequestValidator : AbstractValidator<CreateEventRequest>
{
    public CreateEventRequestValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is required.");

        RuleFor(x => x.Name)
            .NotEmpty().WithMessage("Event name is required.")
            .MaximumLength(100).WithMessage("Event name cannot exceed 100 characters.");

        RuleFor(x => x.Color)
            .NotEmpty().WithMessage("Color is required.")
            .Length(7).WithMessage("Color must be in #RRGGBB format.")
            .Matches("^#([A-Fa-f0-9]{6})$").WithMessage("Color must be a valid hex code in #RRGGBB format.");

        RuleFor(x => x.StartTime)
            .NotEmpty().WithMessage("Start time is required.");

        RuleFor(x => x.EndTime)
            .NotEmpty().WithMessage("End time is required.")
            .GreaterThan(x => x.StartTime).WithMessage("End time must be after start time.");

        RuleFor(x => x.Location)
            .MaximumLength(100).WithMessage("Location cannot exceed 100 characters.")
            .When(x => !string.IsNullOrEmpty(x.Location));

        RuleFor(x => x.Description)
            .MaximumLength(500).WithMessage("Description cannot exceed 500 characters.")
            .When(x => !string.IsNullOrEmpty(x.Description));

        RuleFor(x => x.RecurrenceRule)
            .MaximumLength(100).WithMessage("Recurrence rule cannot exceed 100 characters.")
            .When(x => !string.IsNullOrEmpty(x.RecurrenceRule));
    }
}