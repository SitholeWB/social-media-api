namespace SocialMedia.IntegrationTests;

public class LikesControllerTests(IntegrationTestWebApplicationFactory factory) : BaseControllerTests(factory)
{
    [Fact]
    public async Task LikePost_WithEmoji_ReturnsSuccess()
    {
        var userId = Guid.NewGuid();

        // Create Post
        var createPostDto = new CreatePostDto { Title = "Post to Like", Content = "Content", AuthorId = userId };
        var postResponse = await _client.PostAsJsonAsync($"/api/v1/groups/{Constants.DefaultGroupId}/posts", createPostDto, TestContext.Current.CancellationToken);
        var postId = await postResponse.Content.ReadFromJsonAsync<Guid>(TestContext.Current.CancellationToken);

        // Like Post
        var command = new ToggleLikeCommand(userId, postId, null, "❤️", "Matshana Sithole");
        var response = await _client.PostAsJsonAsync("/api/v1/likes/toggle", command, TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<bool>(TestContext.Current.CancellationToken);
        Assert.True(result); // Added
    }

    [Fact]
    public async Task LikeComment_WithEmoji_ReturnsSuccess()
    {
        var userId = Guid.NewGuid();

        // Create Post & Comment
        var createPostDto = new CreatePostDto { Title = "Post for Comment Like", Content = "Content", AuthorId = userId };
        var postResponse = await _client.PostAsJsonAsync($"/api/v1/groups/{Constants.DefaultGroupId}/posts", createPostDto, TestContext.Current.CancellationToken);
        var postId = await postResponse.Content.ReadFromJsonAsync<Guid>(TestContext.Current.CancellationToken);

        var createCommentDto = new CreateCommentDto { PostId = postId, Content = "Comment to Like", AuthorId = userId };
        var commentResponse = await _client.PostAsJsonAsync("/api/v1/comments", createCommentDto, TestContext.Current.CancellationToken);
        var commentId = await commentResponse.Content.ReadFromJsonAsync<Guid>(TestContext.Current.CancellationToken);

        // Like Comment
        var command = new ToggleLikeCommand(userId, null, commentId, "🔥", "Matshana Sithole");
        var response = await _client.PostAsJsonAsync("/api/v1/likes/toggle", command, TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<bool>(TestContext.Current.CancellationToken);
        Assert.True(result); // Added
    }

    [Fact]
    public async Task LikePost_ShouldReturnNotFound_WhenPostDoesNotExist()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var postId = Guid.NewGuid();

        var command = new ToggleLikeCommand(userId, postId, null, "❤️", "Matshana Sithole");

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/likes/toggle", command, TestContext.Current.CancellationToken);

        // Assert API should return NotFound when post doesn't exist
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task LikeComment_ShouldReturnNotFound_WhenCommentDoesNotExist()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var commentId = Guid.NewGuid();

        var command = new ToggleLikeCommand(userId, null, commentId, "🔥", "Matshana Sithole");

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/likes/toggle", command, TestContext.Current.CancellationToken);

        // Assert API should return NotFound when comment doesn't exist
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }
}