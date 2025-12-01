namespace SocialMedia.IntegrationTests;

public class LikesControllerTests : IClassFixture<IntegrationTestWebApplicationFactory>
{
    private readonly IntegrationTestWebApplicationFactory _factory;

    public LikesControllerTests(IntegrationTestWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task LikePost_WithEmoji_ReturnsSuccess()
    {
        var client = _factory.CreateClient();
        var userId = Guid.NewGuid();

        // Create Post
        var createPostDto = new CreatePostDto { Title = "Post to Like", Content = "Content", AuthorId = userId };
        var postResponse = await client.PostAsJsonAsync("/api/v1/posts", createPostDto, TestContext.Current.CancellationToken);
        var postId = await postResponse.Content.ReadFromJsonAsync<Guid>(TestContext.Current.CancellationToken);

        // Like Post
        var command = new ToggleLikeCommand(userId, postId, null, "❤️");
        var response = await client.PostAsJsonAsync("/api/v1/likes/toggle", command, TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<bool>(TestContext.Current.CancellationToken);
        Assert.True(result); // Added
    }

    [Fact]
    public async Task LikeComment_WithEmoji_ReturnsSuccess()
    {
        var client = _factory.CreateClient();
        var userId = Guid.NewGuid();

        // Create Post & Comment
        var createPostDto = new CreatePostDto { Title = "Post for Comment Like", Content = "Content", AuthorId = userId };
        var postResponse = await client.PostAsJsonAsync("/api/v1/posts", createPostDto, TestContext.Current.CancellationToken);
        var postId = await postResponse.Content.ReadFromJsonAsync<Guid>(TestContext.Current.CancellationToken);

        var createCommentDto = new CreateCommentDto { PostId = postId, Content = "Comment to Like", AuthorId = userId };
        var commentResponse = await client.PostAsJsonAsync("/api/v1/comments", createCommentDto, TestContext.Current.CancellationToken);
        var commentId = await commentResponse.Content.ReadFromJsonAsync<Guid>(TestContext.Current.CancellationToken);

        // Like Comment
        var command = new ToggleLikeCommand(userId, null, commentId, "🔥");
        var response = await client.PostAsJsonAsync("/api/v1/likes/toggle", command, TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<bool>(TestContext.Current.CancellationToken);
        Assert.True(result); // Added
    }
}
