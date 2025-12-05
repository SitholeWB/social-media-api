
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
            await context.SaveChangesAsync(TestContext.Current.CancellationToken);
        }

        var createPostDto = new CreatePostDto
        {
            Title = "Post with Image",
            Content = "Content",
            AuthorId = Guid.NewGuid(),
            FileId = fileId
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/v1/posts", createPostDto, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var postId = await response.Content.ReadFromJsonAsync<Guid>(TestContext.Current.CancellationToken);

        // Process pending events to update read model
        await TestHelpers.ProcessPendingEventsAsync(_factory.Services, TestContext.Current.CancellationToken);

        // Verify retrieval
        var getResponse = await client.GetAsync($"/api/v1/posts", TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
        var result = await getResponse.Content.ReadFromJsonAsync<PagedResult<PostDto>>(TestContext.Current.CancellationToken);
        Assert.NotNull(result);
        var createdPost = result.Items.Find(p => p.Id == postId);
        Assert.NotNull(createdPost);
        Assert.Equal("http://example.com/test.jpg", createdPost.FileUrl);
    }
    [Fact]
    public async Task GetPostById_ShouldReturnNotFound_WhenPostDoesNotExist()
    {
        var client = _factory.CreateClient();
        var postId = Guid.NewGuid();

        var response = await client.GetAsync($"/api/v1/posts/{postId}", TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task ReportPost_ShouldReturnOk_WhenRequestIsValid()
    {
        // Arrange
        var client = _factory.CreateClient();
        var createPostDto = new CreatePostDto { Title = "Post to Report", Content = "Content", AuthorId = Guid.NewGuid() };
        var createResponse = await client.PostAsJsonAsync("/api/v1/posts", createPostDto, TestContext.Current.CancellationToken);
        var postId = await createResponse.Content.ReadFromJsonAsync<Guid>(TestContext.Current.CancellationToken);

        var reportCommand = new ReportPostCommand(postId, Guid.NewGuid()) { Reason = "Spam" };

        // Act
        var response = await client.PostAsJsonAsync($"/api/v1/posts/{postId}/report", reportCommand, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task CreatePost_ShouldReturnCreated_WhenRequestIsValid()
    {
        // Arrange
        var client = _factory.CreateClient();
        var createPostDto = new CreatePostDto { Title = "Test Post", Content = "Test Content", AuthorId = Guid.NewGuid() };

        // Act
        var response = await client.PostAsJsonAsync("/api/v1/posts", createPostDto, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var postId = await response.Content.ReadFromJsonAsync<Guid>(TestContext.Current.CancellationToken);
        Assert.NotEqual(Guid.Empty, postId);
    }
}