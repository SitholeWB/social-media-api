namespace SocialMedia.IntegrationTests;

public class ModerationControllerTests(IntegrationTestWebApplicationFactory factory) : BaseControllerTests(factory)
{
    private async Task<string> RegisterAndLoginAsync(string username, string password, bool isAdmin = false)
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
        return authResponse!.Token;
    }

    [Fact]
    public async Task DeleteReportedContent_ShouldDeletePost_WhenThresholdMet()
    {
        // 1. Setup Users
        var uniqueId = Guid.NewGuid().ToString("N");
        var adminToken = await RegisterAndLoginAsync($"admin_mod_{uniqueId}", "password123", true);
        var userToken = await RegisterAndLoginAsync($"user_mod_{uniqueId}", "password123");

        // 2. Create Post as User
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", userToken);
        var createPostCommand = new CreatePostCommand(new CreatePostDto { Title = "Test Post", Content = "Content", AuthorId = Guid.NewGuid() });
        var createPostResponse = await _client.PostAsJsonAsync("/api/v1/posts", createPostCommand.PostDto, TestContext.Current.CancellationToken);
        createPostResponse.EnsureSuccessStatusCode();
        var postId = await createPostResponse.Content.ReadFromJsonAsync<Guid>(TestContext.Current.CancellationToken);

        // 3. Report Post as User
        var reportCommand = new ReportPostCommand(Guid.Empty, Guid.Empty) { PostId = postId, ReporterId = Guid.NewGuid(), Reason = "Spam" };
        var reportResponse = await _client.PostAsJsonAsync($"/api/v1/posts/{postId}/report", reportCommand, TestContext.Current.CancellationToken);
        reportResponse.EnsureSuccessStatusCode();

        // 4. Delete Reported Content as Admin
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", adminToken);
        var deleteResponse = await _client.DeleteAsync($"/api/v1/moderation/reported-content?minReports=0", TestContext.Current.CancellationToken); // minReports=0 means > 0 reports (so 1 report is enough)

        // Assert
        deleteResponse.EnsureSuccessStatusCode();
        var result = await deleteResponse.Content.ReadFromJsonAsync<Dictionary<string, int>>(TestContext.Current.CancellationToken);
        Assert.True(result!.ContainsKey("deletedCount"));
        Assert.True(result["deletedCount"] >= 1);

        // Verify Post is gone
        var getPostResponse = await _client.GetAsync($"/api/v1/posts/{postId}", TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.NotFound, getPostResponse.StatusCode);
    }

    [Fact]
    public async Task DeleteReportedContent_ShouldReturnForbidden_WhenUserIsNotAdmin()
    {
        // Arrange
        var uniqueId = Guid.NewGuid().ToString("N");
        var userToken = await RegisterAndLoginAsync($"user_mod_fail_{uniqueId}", "password123");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", userToken);

        // Act
        var response = await _client.DeleteAsync($"/api/v1/moderation/reported-content?minReports=1", TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.Forbidden, response.StatusCode);
    }
}