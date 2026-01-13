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

    public virtual async ValueTask InitializeAsync()
    {
        // Runs once before any tests in this class
        var uniqueId = Guid.NewGuid().ToString("N");
        var (token, _) = await RegisterAndLoginAsync($"likeuser_post_{uniqueId}@test.com", "password123");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    protected async Task<(string Token, Guid UserId)> RegisterAndLoginAsync(string username, string password, bool isAdmin = false)
    {
        var email = $"{username}@example.com";
        var registerRequest = new RegisterRequest(username, email, password);
        var registerResponse = await _client.PostAsJsonAsync("/api/v1/auth/register", registerRequest, TestContext.Current.CancellationToken);
        registerResponse.EnsureSuccessStatusCode();

        if (isAdmin)
        {
            using (var scope = _factory.Services.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<SocialMediaDbContext>();
                var user = await dbContext.Users.FirstOrDefaultAsync(u => u.Username == username);
                if (user != null)
                {
                    user.Role = UserRole.Admin;
                    await dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);
                }
            }
        }

        var loginRequest = new LoginRequest(username, password);
        var loginResponse = await _client.PostAsJsonAsync("/api/v1/auth/login", loginRequest, TestContext.Current.CancellationToken);
        loginResponse.EnsureSuccessStatusCode();
        var authResponse = await loginResponse.Content.ReadFromJsonAsync<AuthResponse>(TestContext.Current.CancellationToken);
        return (authResponse!.Token, Guid.Parse(authResponse.Id));
    }
}