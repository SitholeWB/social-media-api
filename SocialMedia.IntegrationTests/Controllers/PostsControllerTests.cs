
namespace SocialMedia.IntegrationTests;

public class PostsControllerTests : IClassFixture<IntegrationTestWebApplicationFactory>
{
    private readonly IntegrationTestWebApplicationFactory _factory;

    public PostsControllerTests(IntegrationTestWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task CreatePost_WithImage_ReturnsUrl()
    {
        // Arrange
        var client = _factory.CreateClient();
        var fileId = Guid.NewGuid();

        // Seed MediaFile
        using (var scope = _factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<SocialMediaDbContext>();
            context.MediaFiles.Add(new MediaFile { Id = fileId, FileName = "test.jpg", Url = "http://example.com/test.jpg" });
            await context.SaveChangesAsync();
        }

        var createPostDto = new CreatePostDto
        {
            Title = "Post with Image",
            Content = "Content",
            AuthorId = Guid.NewGuid(),
            FileId = fileId
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/v1/posts", createPostDto);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var postId = await response.Content.ReadFromJsonAsync<Guid>();

        // Process pending events to update read model
        await TestHelpers.ProcessPendingEventsAsync(_factory.Services);

        // Verify retrieval
        var getResponse = await client.GetAsync($"/api/v1/posts");
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
        var result = await getResponse.Content.ReadFromJsonAsync<PagedResult<PostDto>>();
        Assert.NotNull(result);
        var createdPost = result.Items.Find(p => p.Id == postId);
        Assert.NotNull(createdPost);
        Assert.Equal("http://example.com/test.jpg", createdPost.FileUrl);
    }
}