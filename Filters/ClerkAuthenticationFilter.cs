using System.Security.Claims;
using Canopy.API.Services;

namespace Canopy.API.Filters;

public class ClerkAuthenticationFilter: IEndpointFilter
{
    private readonly ClerkAuthService _clerkAuthService;
    private readonly ILogger<ClerkAuthenticationFilter> _logger;
    
    public ClerkAuthenticationFilter(ClerkAuthService clerkAuthService, ILogger<ClerkAuthenticationFilter> logger)
    {
        _clerkAuthService = clerkAuthService;
        _logger = logger;
    }

    public async ValueTask<object?> InvokeAsync(
        EndpointFilterInvocationContext context,
        EndpointFilterDelegate next)
    {
        _logger.LogInformation("ClerkAuthenticationFilter Invoked");
        var requestState = await _clerkAuthService.AuthenticateRequestAsync(context.HttpContext.Request);
        if (requestState == null || !requestState.IsSignedIn())
        {
            _logger.LogWarning("User is not signed in or authentication failed.");
            return Results.Unauthorized();
        }
        _logger.LogInformation("User is signed in.");
        
        if (requestState.Claims != null)
        {
            var identity = new ClaimsIdentity(requestState.Claims.Claims, "Clerk");
            var principal = new ClaimsPrincipal(identity);
            context.HttpContext.User = principal;
            
        }
        else
        {
            _logger.LogWarning("Authentication successful but no claims found in RequestState.");
            return Results.Unauthorized();
        }
        
        return await next(context);
    }
}