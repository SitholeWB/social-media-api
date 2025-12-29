namespace SocialMedia.IntegrationTests;

public class PostsControllerTests(IntegrationTestWebApplicationFactory factory) : BaseControllerTests(factory)
{
    [Fact]
    public async Task CreatePost_WithImage_ReturnsUrl()
    {
        // Arrange

        // Seed MediaFile

        var createPostDto = new CreatePostDto
        {
            Title = "Post with Image",
            Content = "Content",
            AuthorId = Guid.NewGuid(),
            FileUrl = "http://example.com/test.jpg"
        };

        // Act
        var response = await _client.PostAsJsonAsync($"/api/v1/groups/{Constants.DefaultGroupId}/posts", createPostDto, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var postId = await response.Content.ReadFromJsonAsync<Guid>(TestContext.Current.CancellationToken);

        // Process pending events to update read model
        await TestHelpers.ProcessPendingEventsAsync(_factory.Services, TestContext.Current.CancellationToken);

        // Verify retrieval
        var getResponse = await _client.GetAsync($"/api/v1/groups/{Constants.DefaultGroupId}/posts", TestContext.Current.CancellationToken);
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
        var postId = Guid.NewGuid();

        var response = await _client.GetAsync($"/api/v1/posts/{postId}", TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task ReportPost_ShouldReturnOk_WhenRequestIsValid()
    {
        // Arrange
        var createPostDto = new CreatePostDto { Title = "Post to Report", Content = "Content", AuthorId = Guid.NewGuid() };
        var createResponse = await _client.PostAsJsonAsync($"/api/v1/groups/{Constants.DefaultGroupId}/posts", createPostDto, TestContext.Current.CancellationToken);
        var postId = await createResponse.Content.ReadFromJsonAsync<Guid>(TestContext.Current.CancellationToken);

        var reportCommand = new ReportPostCommand(postId, Guid.NewGuid()) { Reason = "Spam" };

        // Act
        var response = await _client.PostAsJsonAsync($"/api/v1/posts/{postId}/report", reportCommand, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task CreatePost_ShouldReturnCreated_WhenRequestIsValid()
    {
        // Arrange
        var createPostDto = new CreatePostDto { Title = "Test Post", Content = "Test Content", AuthorId = Guid.NewGuid() };

        // Act
        var response = await _client.PostAsJsonAsync($"/api/v1/groups/{Constants.DefaultGroupId}/posts", createPostDto, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var postId = await response.Content.ReadFromJsonAsync<Guid>(TestContext.Current.CancellationToken);
        Assert.NotEqual(Guid.Empty, postId);
    }
}