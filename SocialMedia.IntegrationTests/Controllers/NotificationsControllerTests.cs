using System.Net.Http.Headers;

namespace SocialMedia.IntegrationTests;

public class NotificationsControllerTests : IClassFixture<IntegrationTestWebApplicationFactory>
{
    private readonly IntegrationTestWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public NotificationsControllerTests(IntegrationTestWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    private async Task<(string Token, Guid UserId)> RegisterAndLoginAsync(string username, string password)
    {
        var email = $"{username}@example.com";
        var registerRequest = new RegisterRequest(username, email, password);
        var registerResponse = await _client.PostAsJsonAsync("/api/v1/auth/register", registerRequest);
        registerResponse.EnsureSuccessStatusCode();
        var authResponse = await registerResponse.Content.ReadFromJsonAsync<AuthResponse>();

        var loginRequest = new LoginRequest(username, password);
        var loginResponse = await _client.PostAsJsonAsync("/api/v1/auth/login", loginRequest);
        loginResponse.EnsureSuccessStatusCode();
        var loginAuthResponse = await loginResponse.Content.ReadFromJsonAsync<AuthResponse>();

        return (loginAuthResponse!.Token, Guid.Parse(loginAuthResponse.Id));
    }

    [Fact]
    public async Task GetNotifications_ShouldReturnNotification_WhenPostIsLiked()
    {
        // 1. Setup Users
        var uniqueId = Guid.NewGuid().ToString("N");
        var (user1Token, user1Id) = await RegisterAndLoginAsync($"user1_notif_{uniqueId}", "password123");
        var (user2Token, user2Id) = await RegisterAndLoginAsync($"user2_notif_{uniqueId}", "password123");

        // 2. User 1 Creates Post
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", user1Token);
        var createPostCommand = new CreatePostCommand(new CreatePostDto { Title = "Notif Test Post", Content = "Content", AuthorId = user1Id });
        var createPostResponse = await _client.PostAsJsonAsync("/api/v1/posts", createPostCommand.PostDto);
        createPostResponse.EnsureSuccessStatusCode();
        var postId = await createPostResponse.Content.ReadFromJsonAsync<Guid>();

        // 3. User 2 Likes Post
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", user2Token);
        var toggleLikeCommand = new ToggleLikeCommand(user2Id, postId, null, "👍");
        var likeResponse = await _client.PostAsJsonAsync("/api/v1/likes/toggle", toggleLikeCommand);
        likeResponse.EnsureSuccessStatusCode();

        // 4. User 1 Gets Notifications
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", user1Token);
        var notifResponse = await _client.GetAsync($"/api/v1/notifications/{user1Id}");

        // Assert
        notifResponse.EnsureSuccessStatusCode();
        var notifications = await notifResponse.Content.ReadFromJsonAsync<List<NotificationDto>>();
        Assert.NotNull(notifications);
        Assert.NotEmpty(notifications);
        Assert.Contains(notifications, n => n.RelatedId == postId);
    }
}
