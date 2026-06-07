namespace SocialMedia.Application;

public record LoginRequest(string Username, string Password);
public record GoogleLoginRequest(string IdToken);
public record RegisterRequest(string Username, string Email, string Password)
{
    public string Names { get; set; } = string.Empty;
    public string Surname { get; set; } = string.Empty;
}
public record AuthResponse(string Id, string Username, string Email, string Names, string Surname, string Token, string TenantId);

/// <summary>
/// Supported external OAuth/social login providers.
/// To add a new provider: 1) add an entry here, 2) implement ISocialTokenVerifier,
/// 3) register it in InfrastructureServiceRegistration, 4) add config in appsettings.json.
/// </summary>
public enum SocialProvider
{
    Google,
    Facebook,
    Twitter, // X (Twitter)
    GitHub,
    Apple
}

/// <summary>Generic social login request used by all external OAuth providers.</summary>
public record ExternalLoginRequest(SocialProvider Provider, string AccessToken);