using System.Text.Json;

namespace SocialMedia.Infrastructure;

/// <summary>
/// Verifies a Twitter/X OAuth 2.0 Bearer token by calling the Twitter v2 API.
/// Config path: SocialProviders:Twitter:BearerToken (your app's Bearer Token from developer.twitter.com)
/// The mobile app should pass the user's OAuth2 access_token obtained via PKCE flow.
/// </summary>
public class TwitterTokenVerifier : ISocialTokenVerifier
{
    public SocialProvider Provider => SocialProvider.Twitter;

    private readonly IConfiguration _configuration;
    private readonly HttpClient _httpClient;

    public TwitterTokenVerifier(IConfiguration configuration, IHttpClientFactory httpClientFactory)
    {
        _configuration = configuration;
        _httpClient = httpClientFactory.CreateClient("TwitterApi");
    }

    public async Task<ExternalUserInfo> VerifyAsync(string accessToken, CancellationToken cancellationToken = default)
    {
        // Use the user's OAuth2 access token to call Twitter's /2/users/me endpoint
        var request = new HttpRequestMessage(HttpMethod.Get,
            "https://api.twitter.com/2/users/me?user.fields=id,name,username,public_metrics");
        request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);

        var response = await _httpClient.SendAsync(request, cancellationToken);

        if (!response.IsSuccessStatusCode)
            throw new Exception("Invalid Twitter/X access token.");

        var json = await response.Content.ReadAsStringAsync(cancellationToken);
        var doc = JsonDocument.Parse(json);
        var data = doc.RootElement.GetProperty("data");

        var id = data.GetProperty("id").GetString();
        var name = data.GetProperty("name").GetString() ?? "";
        var username = data.GetProperty("username").GetString() ?? "";

        // Twitter OAuth2 does not return email unless the app has special email permissions.
        // We synthesize an internal email using the username for account linking.
        var email = $"{username}@twitter.social-app.internal";

        var parts = name.Split(' ', 2);
        return new ExternalUserInfo(email, parts[0], parts.Length > 1 ? parts[1] : null, id, SocialProvider.Twitter);
    }
}
