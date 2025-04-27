using Clerk.BackendAPI.Helpers.Jwks;

namespace Canopy.API.Services;

public class ClerkAuthService
{
    private readonly string? _secretKey;
    private readonly string[] _authorizedParties;

    public ClerkAuthService(IConfiguration config)
    {
        _secretKey = config["Clerk:SecretKey"];
        _authorizedParties = config.GetSection("Clerk:AuthorizedParties")
            .Get<string[]>() ?? Array.Empty<string>();
        
        if (string.IsNullOrEmpty(_secretKey))
        {
            throw new InvalidOperationException("Clerk Secret Key not configured.");
        }
    }

public async Task<RequestState?> AuthenticateRequestAsync(HttpRequest request) 
    {
        var options = new AuthenticateRequestOptions(
            secretKey: _secretKey,
            authorizedParties: _authorizedParties
            );

        try
        {
            var requestState = await AuthenticateRequest.AuthenticateRequestAsync(request, options);
            return requestState;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Authentication failed: {ex.Message}");
            return null;
        }
    }
}