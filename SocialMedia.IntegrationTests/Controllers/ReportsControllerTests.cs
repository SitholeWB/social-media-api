using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System.Net.Http.Headers;

namespace SocialMedia.IntegrationTests;

public class ReportsControllerTests : IClassFixture<IntegrationTestWebApplicationFactory>
{
    private readonly IntegrationTestWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public ReportsControllerTests(IntegrationTestWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    private async Task<string> RegisterAndLoginAsync(string username, string password, bool isAdmin = false)
    {
        var email = $"{username}@example.com";
        var registerRequest = new RegisterRequest(username, email, password);
        var registerResponse = await _client.PostAsJsonAsync("/api/v1/auth/register", registerRequest);
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
                    await dbContext.SaveChangesAsync();
                }
            }
        }

        var loginRequest = new LoginRequest(username, password);
        var loginResponse = await _client.PostAsJsonAsync("/api/v1/auth/login", loginRequest);
        loginResponse.EnsureSuccessStatusCode();
        var authResponse = await loginResponse.Content.ReadFromJsonAsync<AuthResponse>();
        return authResponse!.Token;
    }

    [Fact]
    public async Task GetPendingReports_ShouldReturnReports_WhenReportsExist()
    {
        // 1. Setup Users
        var uniqueId = Guid.NewGuid().ToString("N");
        var adminToken = await RegisterAndLoginAsync($"admin_reports_{uniqueId}", "password123", true);
        var userToken = await RegisterAndLoginAsync($"user_reports_{uniqueId}", "password123");

        // 2. Create Post as User
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", userToken);
        var createPostCommand = new CreatePostCommand(new CreatePostDto { Title = "Report Test Post", Content = "Content", AuthorId = Guid.NewGuid() });
        var createPostResponse = await _client.PostAsJsonAsync("/api/v1/posts", createPostCommand.PostDto);
        createPostResponse.EnsureSuccessStatusCode();
        var postId = await createPostResponse.Content.ReadFromJsonAsync<Guid>();

        // 3. Report Post as User
        var reportCommand = new ReportPostCommand(Guid.Empty, Guid.Empty) { PostId = postId, ReporterId = Guid.NewGuid(), Reason = "Inappropriate content" };
        var reportResponse = await _client.PostAsJsonAsync($"/api/v1/posts/{postId}/report", reportCommand);
        reportResponse.EnsureSuccessStatusCode();

        // 4. Get Pending Reports as Admin
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", adminToken);
        var getReportsResponse = await _client.GetAsync("/api/v1/reports/pending");

        // Assert
        getReportsResponse.EnsureSuccessStatusCode();
        var result = await getReportsResponse.Content.ReadFromJsonAsync<PagedResult<ReportDto>>();
        Assert.NotNull(result);
        Assert.NotEmpty(result.Items);
        Assert.Contains(result.Items, r => r.PostId == postId);
    }
}