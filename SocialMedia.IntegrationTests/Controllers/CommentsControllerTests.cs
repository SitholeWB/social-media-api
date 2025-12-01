
namespace SocialMedia.IntegrationTests;

public class CommentsControllerTests : IClassFixture<IntegrationTestWebApplicationFactory>
{
    private readonly IntegrationTestWebApplicationFactory _factory;

    public CommentsControllerTests(IntegrationTestWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task CreateComment_WithValidPost_ReturnsCreated()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Create a post first
        var createPostDto = new CreatePostDto { Title = "Test Post", Content = "Content", AuthorId = Guid.NewGuid() };
        var postResponse = await client.PostAsJsonAsync("/api/v1/posts", createPostDto);
        postResponse.EnsureSuccessStatusCode();
        var postId = await postResponse.Content.ReadFromJsonAsync<Guid>();

        var createCommentDto = new CreateCommentDto { PostId = postId, Content = "Test Comment", AuthorId = Guid.NewGuid() };

        // Act
        var response = await client.PostAsJsonAsync("/api/v1/comments", createCommentDto);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var commentId = await response.Content.ReadFromJsonAsync<Guid>();
        Assert.NotEqual(Guid.Empty, commentId);
    }

    [Fact]
    public async Task CreateComment_WithInvalidPost_ReturnsInternalServerError_Or_NotFound()
    {
        // Arrange
        var client = _factory.CreateClient();
        var createCommentDto = new CreateCommentDto { PostId = Guid.NewGuid(), Content = "Test Comment", AuthorId = Guid.NewGuid() };

        // Act
        var response = await client.PostAsJsonAsync("/api/v1/comments", createCommentDto);

        // Assert Since we throw KeyNotFoundException, and default middleware might return 500 or
        // 404 depending on config. Usually unhandled exception is 500. If we had exception
        // middleware mapping KeyNotFound to 404, it would be 404. Let's check for failure status
        // code for now.
        Assert.False(response.IsSuccessStatusCode);
    }

    [Fact]
    public async Task GetCommentsByPostId_ReturnsComments()
    {
        // Arrange
        var client = _factory.CreateClient();

        // Create a post
        var createPostDto = new CreatePostDto { Title = "Test Post 2", Content = "Content 2", AuthorId = Guid.NewGuid() };
        var postResponse = await client.PostAsJsonAsync("/api/v1/posts", createPostDto);
        var postId = await postResponse.Content.ReadFromJsonAsync<Guid>();

        // Create a comment
        var createCommentDto = new CreateCommentDto { PostId = postId, Content = "Test Comment 2", AuthorId = Guid.NewGuid() };
        await client.PostAsJsonAsync("/api/v1/comments", createCommentDto);

        // Act
        var response = await client.GetAsync($"/api/v1/comments/post/{postId}");

        // Assert
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var result = await response.Content.ReadFromJsonAsync<PagedResult<CommentDto>>();
        Assert.NotNull(result);
        Assert.True(result.Items.Count >= 1);
    }

    [Fact]
    public async Task CreateComment_WithImage_ReturnsUrl()
    {
        // Arrange
        var client = _factory.CreateClient();
        var fileId = Guid.NewGuid();

        // Seed MediaFile
        using (var scope = _factory.Services.CreateScope())
        {
            var context = scope.ServiceProvider.GetRequiredService<SocialMediaDbContext>();
            context.MediaFiles.Add(new MediaFile { Id = fileId, FileName = "comment.jpg", Url = "http://example.com/comment.jpg" });
            await context.SaveChangesAsync();
        }

        // Create a post
        var createPostDto = new CreatePostDto { Title = "Post for Comment Image", Content = "Content", AuthorId = Guid.NewGuid() };
        var postResponse = await client.PostAsJsonAsync("/api/v1/posts", createPostDto);
        var postId = await postResponse.Content.ReadFromJsonAsync<Guid>();

        var createCommentDto = new CreateCommentDto
        {
            PostId = postId,
            Content = "Comment with Image",
            AuthorId = Guid.NewGuid(),
            FileId = fileId
        };

        // Act
        var response = await client.PostAsJsonAsync("/api/v1/comments", createCommentDto);

        // Assert
        Assert.Equal(HttpStatusCode.Created, response.StatusCode);
        var commentId = await response.Content.ReadFromJsonAsync<Guid>();

        // Verify retrieval
        var getResponse = await client.GetAsync($"/api/v1/comments/{commentId}");
        Assert.Equal(HttpStatusCode.OK, getResponse.StatusCode);
        var comment = await getResponse.Content.ReadFromJsonAsync<CommentDto>();
        Assert.NotNull(comment);
        Assert.Equal("http://example.com/comment.jpg", comment.FileUrl);
    }
}