using Google.Apis.Auth;
using SocialMedia.Application;

namespace SocialMedia.Infrastructure;

/// <summary>
/// Verifies a Google sign-in identity token (idToken JWT) using Google's validation SDK.
/// </summary>
public class GoogleTokenVerifier : ISocialTokenVerifier
{
    public SocialProvider Provider => SocialProvider.Google;

    public async Task<ExternalUserInfo> VerifyAsync(string accessToken, CancellationToken cancellationToken = default)
    {
        try
        {
            // The accessToken passed from the frontend is the ID token (id_token JWT)
            var payload = await GoogleJsonWebSignature.ValidateAsync(accessToken);
            
            var email = payload.Email;
            var givenName = payload.GivenName;
            var familyName = payload.FamilyName;
            var sub = payload.Subject;

            if (string.IsNullOrWhiteSpace(email))
                throw new Exception("Google token does not contain a verified email address.");

            return new ExternalUserInfo(email, givenName, familyName, sub, SocialProvider.Google);
        }
        catch (InvalidJwtException ex)
        {
            throw new Exception($"Invalid Google token: {ex.Message}");
        }
    }
}
