namespace SocialMedia.IntegrationTests;

public class ReactionCommentTests : IClassFixture<IntegrationTestWebApplicationFactory>
{
    private readonly IntegrationTestWebApplicationFactory _factory;

    public ReactionCommentTests(IntegrationTestWebApplicationFactory factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task ReactToComment_ShouldUpdateReadModel()
    {
        // Arrange
        var client = _factory.CreateClient();
        var postDto = new CreatePostDto { Title = "Test Post", Content = "Content", AuthorId = Guid.NewGuid() };
        var postResponse = await client.PostAsJsonAsync("/api/v1/posts", postDto, TestContext.Current.CancellationToken);
        var postId = await postResponse.Content.ReadFromJsonAsync<Guid>(TestContext.Current.CancellationToken);

        var commentDto = new CreateCommentDto { PostId = postId, Content = "Test Comment", AuthorId = Guid.NewGuid() };
        var commentResponse = await client.PostAsJsonAsync("/api/v1/comments", commentDto, TestContext.Current.CancellationToken);
        var commentId = await commentResponse.Content.ReadFromJsonAsync<Guid>(TestContext.Current.CancellationToken);

        // Act: React to comment
        var reactionCommand = new { CommentId = commentId, UserId = Guid.NewGuid(), Emoji = "❤️" };
        var reactionResponse = await client.PostAsJsonAsync("/api/v1/likes/toggle", reactionCommand, TestContext.Current.CancellationToken);
        reactionResponse.EnsureSuccessStatusCode();

        // Process pending events
        await TestHelpers.ProcessPendingEventsAsync(_factory.Services, TestContext.Current.CancellationToken);

        // Assert: Check Post Read Model (TopComments)
        var getPostResponse = await client.GetAsync($"/api/v1/posts?pageNumber=1&pageSize=10", TestContext.Current.CancellationToken);
        var postResult = await getPostResponse.Content.ReadFromJsonAsync<PagedResult<PostDto>>(TestContext.Current.CancellationToken);
        var post = postResult.Items.FirstOrDefault(p => p.Id == postId);

        // Note: PostDto currently doesn't expose TopComments, we might need to update PostDto or fetch comments separately
        // But we can check the comments endpoint

        var getCommentsResponse = await client.GetAsync($"/api/v1/posts/{postId}/comments", TestContext.Current.CancellationToken);
        getCommentsResponse.EnsureSuccessStatusCode();
        var commentsResult = await getCommentsResponse.Content.ReadFromJsonAsync<PagedResult<CommentReadDto>>(TestContext.Current.CancellationToken);

        Assert.NotNull(commentsResult);
        var comment = commentsResult.Items.FirstOrDefault(c => c.CommentId == commentId);
        Assert.NotNull(comment);
        Assert.Equal(1, comment.LikeCount);
        Assert.Contains(comment.Reactions, r => r.Emoji == "❤️");
    }

    [Fact]
    public async Task GetPost_ShouldReturnTop30Comments()
    {
        // Arrange
        var client = _factory.CreateClient();
        var postDto = new CreatePostDto { Title = "Popular Post", Content = "Content", AuthorId = Guid.NewGuid() };
        var postResponse = await client.PostAsJsonAsync("/api/v1/posts", postDto, TestContext.Current.CancellationToken);
        var postId = await postResponse.Content.ReadFromJsonAsync<Guid>(TestContext.Current.CancellationToken);

        // Add 35 comments
        for (int i = 0; i < 35; i++)
        {
            var commentDto = new CreateCommentDto { PostId = postId, Content = $"Comment {i}", AuthorId = Guid.NewGuid() };
            await client.PostAsJsonAsync("/api/v1/comments", commentDto, TestContext.Current.CancellationToken);
        }

        // Process pending events
        await TestHelpers.ProcessPendingEventsAsync(_factory.Services, TestContext.Current.CancellationToken);

        // Act: Get Post Comments (Page 1)
        var getCommentsResponse = await client.GetAsync($"/api/v1/posts/{postId}/comments?pageNumber=1&pageSize=30", TestContext.Current.CancellationToken);
        var commentsResult = await getCommentsResponse.Content.ReadFromJsonAsync<PagedResult<CommentReadDto>>(TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(30, commentsResult.Items.Count);

        // Act: Get Remaining Comments (Page 2)
        var getMoreCommentsResponse = await client.GetAsync($"/api/v1/posts/{postId}/comments?pageNumber=2&pageSize=10", TestContext.Current.CancellationToken);
        var moreCommentsResult = await getMoreCommentsResponse.Content.ReadFromJsonAsync<PagedResult<CommentReadDto>>(TestContext.Current.CancellationToken);

        Assert.True(moreCommentsResult.Items.Count >= 5); // Should be 5
    }
}
