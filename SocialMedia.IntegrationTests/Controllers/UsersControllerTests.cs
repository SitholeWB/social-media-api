
namespace SocialMedia.IntegrationTests;

public class UsersControllerTests : IClassFixture<IntegrationTestWebApplicationFactory>
{
    private readonly IntegrationTestWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public UsersControllerTests(IntegrationTestWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    private async Task<(string Token, Guid UserId)> RegisterAndLoginAsync(string username, string password, bool isAdmin = false)
    {
        var email = $"{username}@example.com";
        var registerRequest = new RegisterRequest(username, email, password);
        var registerResponse = await _client.PostAsJsonAsync("/api/v1/auth/register", registerRequest, TestContext.Current.CancellationToken);
        registerResponse.EnsureSuccessStatusCode();
        var registerAuthResponse = await registerResponse.Content.ReadFromJsonAsync<AuthResponse>(TestContext.Current.CancellationToken);

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

    [Fact]
    public async Task BlockUser_ShouldReturnOk_WhenUserBlocksAnother()
    {
        // 1. Setup Users
        var uniqueId = Guid.NewGuid().ToString("N");
        var (user1Token, user1Id) = await RegisterAndLoginAsync($"user1_block_{uniqueId}", "password123");
        var (user2Token, user2Id) = await RegisterAndLoginAsync($"user2_block_{uniqueId}", "password123");

        // 2. User 1 Blocks User 2
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", user1Token);
        var blockCommand = new BlockUserCommand(user1Id, user2Id);
        var response = await _client.PostAsJsonAsync("/api/v1/users/block", blockCommand, TestContext.Current.CancellationToken);

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task BanUser_ShouldPreventLogin_WhenAdminBansUser()
    {
        // 1. Setup Users
        var uniqueId = Guid.NewGuid().ToString("N");
        var (adminToken, _) = await RegisterAndLoginAsync($"admin_ban_{uniqueId}", "password123", true);
        var (userToken, userId) = await RegisterAndLoginAsync($"user_ban_{uniqueId}", "password123");

        // 2. Admin Bans User
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", adminToken);
        var response = await _client.PostAsJsonAsync($"/api/v1/users/{userId}/ban", true, TestContext.Current.CancellationToken);
        response.EnsureSuccessStatusCode();

        // 3. User Tries to Login
        var loginRequest = new LoginRequest($"user_ban_{uniqueId}", "password123");
        var loginResponse = await _client.PostAsJsonAsync("/api/v1/auth/login", loginRequest, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.Unauthorized, loginResponse.StatusCode);
    }
}