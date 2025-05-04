using Canopy.API.Models.Events;
using FluentValidation;

namespace Canopy.API.Validators.Events;

public class GetMonthlyEventsQueryValidator : AbstractValidator<GetMonthlyEventsQuery>
{
    public GetMonthlyEventsQueryValidator()
    {
        RuleFor(x => x.Year)
            .GreaterThan(0).WithMessage("Year must be a positive integer.");

        RuleFor(x => x.Month)
            .InclusiveBetween(1, 12).WithMessage("Month must be between 1 and 12.");

        RuleFor(x => x.Timezone)
            .NotEmpty().WithMessage("Timezone is required.");
    }
}