using FluentValidation;

namespace Canopy.API.Filters;

public class ValidationFilter<T> : IEndpointFilter where T : class
{
    private readonly IValidator<T> _validator;

    public ValidationFilter(IValidator<T> validator)
    {
        _validator = validator ?? throw new ArgumentNullException(nameof(validator));
    }

    public async ValueTask<object?> InvokeAsync(
        EndpointFilterInvocationContext context,
        EndpointFilterDelegate next)
    {
        T? argumentToValidate = context.GetArgument<T>(
            context.Arguments.ToList().FindIndex(a => a?.GetType() == typeof(T))
        );

        if (argumentToValidate is null)
        {
            return Results.Problem("Could not find type to validate", statusCode: 400);
        }

        var validationResult = await _validator.ValidateAsync(argumentToValidate);

        if (!validationResult.IsValid)
        {
            return Results.ValidationProblem(validationResult.ToDictionary());
        }

        return await next(context);
    }
}