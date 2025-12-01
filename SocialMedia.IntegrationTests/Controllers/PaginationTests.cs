namespace SocialMedia.IntegrationTests;

public class PaginationTests : IClassFixture<IntegrationTestWebApplicationFactory>
{
    private readonly HttpClient _client;
    private readonly IntegrationTestWebApplicationFactory _factory;

    public PaginationTests(IntegrationTestWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task GetPosts_ShouldReturnPagedResult()
    {
        // Arrange: Create some posts
        for (int i = 0; i < 15; i++)
        {
            var command = new CreatePostCommand(new CreatePostDto { Content = $"Post {i}", AuthorId = Guid.NewGuid() });
            await _client.PostAsJsonAsync("/api/v1/posts", command);
        }

        // Process pending events
        await TestHelpers.ProcessPendingEventsAsync(_factory.Services);

        // Act: Get Page 1
        var response1 = await _client.GetAsync("/api/v1/posts?pageNumber=1&pageSize=10");
        response1.EnsureSuccessStatusCode();
        var result1 = await response1.Content.ReadFromJsonAsync<PagedResult<PostDto>>();

        // Act: Get Page 2
        var response2 = await _client.GetAsync("/api/v1/posts?pageNumber=2&pageSize=10");
        response2.EnsureSuccessStatusCode();
        var result2 = await response2.Content.ReadFromJsonAsync<PagedResult<PostDto>>();

        // Assert
        Assert.NotNull(result1);
        Assert.Equal(10, result1.Items.Count);
        Assert.Equal(1, result1.PageNumber);
        Assert.True(result1.TotalCount >= 15);

        Assert.NotNull(result2);
        Assert.True(result2.Items.Count >= 5);
        Assert.Equal(2, result2.PageNumber);
    }
}
