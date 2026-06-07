using System.Text.Json;

namespace SocialMedia.Infrastructure;

/// <summary>
/// Verifies a Facebook user access token by calling the Facebook Graph API.
/// Config path: SocialProviders:Facebook:AppId and SocialProviders:Facebook:AppSecret
/// </summary>
public class FacebookTokenVerifier : ISocialTokenVerifier
{
    public SocialProvider Provider => SocialProvider.Facebook;

    private readonly IConfiguration _configuration;
    private readonly HttpClient _httpClient;

    public FacebookTokenVerifier(IConfiguration configuration, IHttpClientFactory httpClientFactory)
    {
        _configuration = configuration;
        _httpClient = httpClientFactory.CreateClient("FacebookGraph");
    }

    public async Task<ExternalUserInfo> VerifyAsync(string accessToken, CancellationToken cancellationToken = default)
    {
        var appId = _configuration["SocialProviders:Facebook:AppId"]
            ?? throw new InvalidOperationException("Facebook AppId is not configured.");
        var appSecret = _configuration["SocialProviders:Facebook:AppSecret"]
            ?? throw new InvalidOperationException("Facebook AppSecret is not configured.");

        // Step 1: Validate token via debug_token endpoint
        var appToken = $"{appId}|{appSecret}";
        var debugUrl = $"https://graph.facebook.com/debug_token?input_token={accessToken}&access_token={appToken}";
        var debugResponse = await _httpClient.GetAsync(debugUrl, cancellationToken);
        debugResponse.EnsureSuccessStatusCode();

        var debugJson = await debugResponse.Content.ReadAsStringAsync(cancellationToken);
        var debugDoc = JsonDocument.Parse(debugJson);
        var data = debugDoc.RootElement.GetProperty("data");

        if (!data.GetProperty("is_valid").GetBoolean())
            throw new Exception("Invalid Facebook access token.");

        // Step 2: Fetch user profile
        var profileUrl = $"https://graph.facebook.com/me?fields=id,email,first_name,last_name&access_token={accessToken}";
        var profileResponse = await _httpClient.GetAsync(profileUrl, cancellationToken);
        profileResponse.EnsureSuccessStatusCode();

        var profileJson = await profileResponse.Content.ReadAsStringAsync(cancellationToken);
        var profile = JsonDocument.Parse(profileJson).RootElement;

        var email = profile.TryGetProperty("email", out var emailEl) ? emailEl.GetString() : null;
        var firstName = profile.TryGetProperty("first_name", out var fnEl) ? fnEl.GetString() : null;
        var lastName = profile.TryGetProperty("last_name", out var lnEl) ? lnEl.GetString() : null;
        var id = profile.GetProperty("id").GetString();

        if (string.IsNullOrWhiteSpace(email))
            throw new Exception("Facebook account does not have a verified email address.");

        return new ExternalUserInfo(email, firstName, lastName, id, SocialProvider.Facebook);
    }
}
