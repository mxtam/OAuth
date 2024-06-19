using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Primitives;
using OpenIddict.Abstractions;
using System.Security.Claims;


namespace OAuth.Services;

public class AuthorizationService
{
    //Метод для парсингу параметрів з форми або з рядка запиту
    public IDictionary<string, StringValues> ParseOAuthParameters(HttpContext httpContext, List<string>? excluding = null)
    {
        excluding ??= new List<string>();

        var parameters = httpContext.Request.HasFormContentType
            ? httpContext.Request.Form
                .Where(v => !excluding.Contains(v.Key))
                    .ToDictionary(v => v.Key, v => v.Value)
            : httpContext.Request.Query
                .Where(v => !excluding.Contains(v.Key))
                    .ToDictionary(v => v.Key, v => v.Value);

        return parameters;

    }
    //Метод для конструювання посилання для переанправлення
    public string BuildRedirectUrl(HttpRequest httpRequest, IDictionary<string, StringValues> oauthParameters)
    {
        var url = httpRequest.PathBase + httpRequest.Path + QueryString.Create(oauthParameters);
        return url;
    }

    //Метод для перевірки автентифікації
    public bool IsAuthenticated(AuthenticateResult authenticateResult, OpenIddictRequest request)
    {
        if (!authenticateResult.Succeeded)
        {
            return false;
        }

        if (request.MaxAge.HasValue && authenticateResult.Properties != null)
        {
            var maxAgeSeconds = TimeSpan.FromSeconds(request.MaxAge.Value);

            var expired = !authenticateResult.Properties.IssuedUtc.HasValue ||
                            DateTimeOffset.UtcNow - authenticateResult.Properties.IssuedUtc > maxAgeSeconds;

            if (expired)
            {
                return false;
            }
        }

        return true;
    }
    //В цьому методі ми додаємо клейми нашого користувача до access token
    public static List<string> GetDestinations(ClaimsIdentity identity, Claim claim)
    {
        var destinations = new List<string>();

        if (claim.Type is OpenIddictConstants.Claims.Name or OpenIddictConstants.Claims.Email)
        {
            destinations.Add(OpenIddictConstants.Destinations.AccessToken);

            if (identity.HasScope(OpenIddictConstants.Scopes.OpenId))
            {
                destinations.Add(OpenIddictConstants.Destinations.IdentityToken);
            }
        }

        return destinations;
    }
}
