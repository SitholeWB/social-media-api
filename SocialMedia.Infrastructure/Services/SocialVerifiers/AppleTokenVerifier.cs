using System.IdentityModel.Tokens.Jwt;
using System.Text.Json;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;

namespace SocialMedia.Infrastructure;

/// <summary>
/// Verifies a Sign in with Apple identity token (id_token JWT).
/// The token is validated against Apple's public keys fetched from:
///   https://appleid.apple.com/auth/keys
///
/// Config path: SocialProviders:Apple:ClientId  (your Services ID, e.g. "com.yourapp.service")
/// 
/// HOW TO SET UP
/// ─────────────
/// 1. In the Apple Developer Portal, create a "Services ID" under Certificates, Identifiers & Profiles.
/// 2. Enable "Sign in with Apple" on the Services ID and configure your return URLs.
/// 3. Set SocialProviders:Apple:ClientId in appsettings.json (or secrets) to your Services ID.
/// 4. On iOS the native Sign in with Apple returns an id_token directly — pass it as the AccessToken.
/// </summary>
public class AppleTokenVerifier : ISocialTokenVerifier
{
    public SocialProvider Provider => SocialProvider.Apple;

    private readonly IConfiguration _configuration;

    // Apple's OIDC discovery endpoint — keys are cached automatically
    private const string AppleJwksUri = "https://appleid.apple.com/auth/keys";
    private const string AppleIssuer = "https://appleid.apple.com";

    public AppleTokenVerifier(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<ExternalUserInfo> VerifyAsync(string idToken, CancellationToken cancellationToken = default)
    {
        var clientId = _configuration["SocialProviders:Apple:ClientId"]
            ?? throw new InvalidOperationException("SocialProviders:Apple:ClientId is not configured.");

        // Fetch Apple's public signing keys
        var configManager = new ConfigurationManager<OpenIdConnectConfiguration>(
            $"https://appleid.apple.com/.well-known/openid-configuration",
            new OpenIdConnectConfigurationRetriever(),
            new HttpDocumentRetriever());

        var oidcConfig = await configManager.GetConfigurationAsync(cancellationToken);

        var validationParams = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidIssuer = AppleIssuer,
            ValidateAudience = true,
            ValidAudience = clientId,
            ValidateLifetime = true,
            IssuerSigningKeys = oidcConfig.SigningKeys,
        };

        var handler = new JwtSecurityTokenHandler();
        ClaimsPrincipal principal;
        try
        {
            principal = handler.ValidateToken(idToken, validationParams, out _);
        }
        catch (Exception ex)
        {
            throw new Exception($"Invalid Apple identity token: {ex.Message}");
        }

        // Extract claims from the validated JWT
        var sub = principal.FindFirst(System.Security.Claims.ClaimTypes.NameIdentifier)?.Value
                  ?? principal.FindFirst("sub")?.Value
                  ?? throw new Exception("Apple token missing 'sub' claim.");

        var email = principal.FindFirst(System.Security.Claims.ClaimTypes.Email)?.Value
                    ?? principal.FindFirst("email")?.Value
                    ?? $"{sub}@appleid.social-app.internal"; // Apple may hide email

        // Apple may provide name on first login only
        var givenName = principal.FindFirst(System.Security.Claims.ClaimTypes.GivenName)?.Value ?? "Apple";
        var familyName = principal.FindFirst(System.Security.Claims.ClaimTypes.Surname)?.Value ?? "User";

        return new ExternalUserInfo(email, givenName, familyName, sub, SocialProvider.Apple);
    }
}
