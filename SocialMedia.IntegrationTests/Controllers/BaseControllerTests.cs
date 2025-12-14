namespace SocialMedia.IntegrationTests;

public class BaseControllerTests : IClassFixture<IntegrationTestWebApplicationFactory>, IAsyncLifetime

{
    protected readonly IntegrationTestWebApplicationFactory _factory;
    protected readonly HttpClient _client;

    public BaseControllerTests(IntegrationTestWebApplicationFactory factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    public ValueTask DisposeAsync()
    {
        return ValueTask.CompletedTask;
    }

    public async ValueTask InitializeAsync()
    {
        // Runs once before any tests in this class
        var uniqueId = Guid.NewGuid().ToString("N");
        var token = await RegisterAndLoginAsyncx($"likeuser_post_{uniqueId}@test.com", "password123");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    protected async Task<string> RegisterAndLoginAsyncx(string email, string password)
    {
        var registerRequest = new { Email = email, Password = password, Username = email.Split('@')[0] };
        await _client.PostAsJsonAsync("/api/v1/auth/register", registerRequest, TestContext.Current.CancellationToken);
        var loginRequest = new { Username = email, Password = password };
        var loginResponse = await _client.PostAsJsonAsync("/api/v1/auth/login", loginRequest, TestContext.Current.CancellationToken);
        var authResponse = await loginResponse.Content.ReadFromJsonAsync<AuthResponse>(TestContext.Current.CancellationToken);
        return authResponse!.Token;
    }
}