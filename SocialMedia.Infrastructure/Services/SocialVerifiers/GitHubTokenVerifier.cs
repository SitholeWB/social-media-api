using System.Text.Json;

namespace SocialMedia.Infrastructure;

/// <summary>
/// Verifies a GitHub OAuth access token by calling the GitHub API.
/// Config path: SocialProviders:GitHub:ClientId and SocialProviders:GitHub:ClientSecret
/// </summary>
public class GitHubTokenVerifier : ISocialTokenVerifier
{
    public SocialProvider Provider => SocialProvider.GitHub;

    private readonly IConfiguration _configuration;
    private readonly HttpClient _httpClient;

    public GitHubTokenVerifier(IConfiguration configuration, IHttpClientFactory httpClientFactory)
    {
        _configuration = configuration;
        _httpClient = httpClientFactory.CreateClient("GitHubApi");
    }

    public async Task<ExternalUserInfo> VerifyAsync(string accessToken, CancellationToken cancellationToken = default)
    {
        // Fetch user profile
        var profileReq = new HttpRequestMessage(HttpMethod.Get, "https://api.github.com/user");
        profileReq.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
        profileReq.Headers.UserAgent.ParseAdd("SocialMedia-App/1.0");

        var profileResp = await _httpClient.SendAsync(profileReq, cancellationToken);
        if (!profileResp.IsSuccessStatusCode)
            throw new Exception("Invalid GitHub access token.");

        var profileJson = await profileResp.Content.ReadAsStringAsync(cancellationToken);
        var profile = JsonDocument.Parse(profileJson).RootElement;

        // Fetch primary email separately (GitHub requires a separate emails call)
        var emailReq = new HttpRequestMessage(HttpMethod.Get, "https://api.github.com/user/emails");
        emailReq.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", accessToken);
        emailReq.Headers.UserAgent.ParseAdd("SocialMedia-App/1.0");

        var emailResp = await _httpClient.SendAsync(emailReq, cancellationToken);
        string? email = null;
        if (emailResp.IsSuccessStatusCode)
        {
            var emailJson = await emailResp.Content.ReadAsStringAsync(cancellationToken);
            var emails = JsonDocument.Parse(emailJson).RootElement;
            foreach (var e in emails.EnumerateArray())
            {
                if (e.TryGetProperty("primary", out var primary) && primary.GetBoolean()
                    && e.TryGetProperty("verified", out var verified) && verified.GetBoolean())
                {
                    email = e.GetProperty("email").GetString();
                    break;
                }
            }
        }

        var login = profile.GetProperty("login").GetString() ?? "";
        var name = profile.TryGetProperty("name", out var nameEl) ? nameEl.GetString() ?? login : login;
        var id = profile.GetProperty("id").ToString();

        // Fallback: synthesize email from login if no public/verified email
        email ??= $"{login}@github.social-app.internal";

        var parts = name.Split(' ', 2);
        return new ExternalUserInfo(email, parts[0], parts.Length > 1 ? parts[1] : null, id, SocialProvider.GitHub);
    }
}
