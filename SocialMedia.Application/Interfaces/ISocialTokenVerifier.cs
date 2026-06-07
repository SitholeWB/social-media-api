namespace SocialMedia.Application;

/// <summary>
/// Normalized user info returned by any social provider after token verification.
/// </summary>
public record ExternalUserInfo(
    string Email,
    string? GivenName,
    string? FamilyName,
    string? ProviderId,
    SocialProvider Provider
);

/// <summary>
/// Contract for verifying an OAuth access/ID token from a specific social provider
/// and extracting the user's profile information.
/// Implement this interface to add support for a new social login provider.
/// </summary>
public interface ISocialTokenVerifier
{
    SocialProvider Provider { get; }

    /// <summary>
    /// Verifies the token and returns user info, or throws if the token is invalid.
    /// </summary>
    Task<ExternalUserInfo> VerifyAsync(string accessToken, CancellationToken cancellationToken = default);
}
