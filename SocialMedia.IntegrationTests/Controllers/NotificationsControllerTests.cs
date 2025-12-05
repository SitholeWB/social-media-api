
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
        var registerResponse = await _client.PostAsJsonAsync("/api/v1/auth/register", registerRequest, TestContext.Current.CancellationToken);
        registerResponse.EnsureSuccessStatusCode();
        var authResponse = await registerResponse.Content.ReadFromJsonAsync<AuthResponse>(TestContext.Current.CancellationToken);

        var loginRequest = new LoginRequest(username, password);
        var loginResponse = await _client.PostAsJsonAsync("/api/v1/auth/login", loginRequest, TestContext.Current.CancellationToken);
        loginResponse.EnsureSuccessStatusCode();
        var loginAuthResponse = await loginResponse.Content.ReadFromJsonAsync<AuthResponse>(TestContext.Current.CancellationToken);

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
        var createPostResponse = await _client.PostAsJsonAsync("/api/v1/posts", createPostCommand.PostDto, TestContext.Current.CancellationToken);
        createPostResponse.EnsureSuccessStatusCode();
        var postId = await createPostResponse.Content.ReadFromJsonAsync<Guid>(TestContext.Current.CancellationToken);

        // 3. User 2 Likes Post
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", user2Token);
        var toggleLikeCommand = new ToggleLikeCommand(user2Id, postId, null, "👍");
        var likeResponse = await _client.PostAsJsonAsync("/api/v1/likes/toggle", toggleLikeCommand, TestContext.Current.CancellationToken);
        likeResponse.EnsureSuccessStatusCode();

        // 4. User 1 Gets Notifications
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", user1Token);
        var notifResponse = await _client.GetAsync($"/api/v1/notifications/{user1Id}", TestContext.Current.CancellationToken);

        // Assert
        notifResponse.EnsureSuccessStatusCode();
        var notifications = await notifResponse.Content.ReadFromJsonAsync<List<NotificationDto>>(TestContext.Current.CancellationToken);
        Assert.NotNull(notifications);
        Assert.NotEmpty(notifications);
        Assert.Contains(notifications, n => n.RelatedId == postId);
    }
    [Fact]
    public async Task GetNotifications_ShouldReturnEmpty_WhenNoNotificationsExist()
    {
        // Arrange
        var uniqueId = Guid.NewGuid().ToString("N");
        var (userToken, userId) = await RegisterAndLoginAsync($"user_no_notif_{uniqueId}", "password123");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", userToken);

        // Act
        var response = await _client.GetAsync($"/api/v1/notifications/{userId}", TestContext.Current.CancellationToken);

        // Assert
        response.EnsureSuccessStatusCode();
        var notifications = await response.Content.ReadFromJsonAsync<List<NotificationDto>>(TestContext.Current.CancellationToken);
        Assert.NotNull(notifications);
        Assert.Empty(notifications);
    }
}
