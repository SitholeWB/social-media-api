namespace SocialMedia.IntegrationTests;

public class StatsIntegrationTests(IntegrationTestWebApplicationFactory factory) : BaseControllerTests(factory)
{
    [Fact]
    public async Task Stats_ShouldTrack_PostCreation_Comments_And_Reactions()
    {
        // Arrange
        var authorId = Guid.NewGuid();
        var createPostDto = new CreatePostDto
        {
            Title = "Stats Test Post",
            Content = "Testing Stats",
            AuthorId = authorId
        };

        // --- 1. Create Post ---
        var postResponse = await _client.PostAsJsonAsync($"/api/v1/groups/{Constants.DefaultGroupId}/posts", createPostDto, TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.Created, postResponse.StatusCode);
        var postId = await postResponse.Content.ReadFromJsonAsync<Guid>(TestContext.Current.CancellationToken);

        // Process Events
        await TestHelpers.ProcessPendingEventsAsync(_factory.Services, TestContext.Current.CancellationToken);

        // Verify Weekly Stats (NewPosts should be 1, TotalPosts >= 1)
        var statsResponse = await _client.GetAsync("/api/v1/stats/weekly", TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.OK, statsResponse.StatusCode);
        var stats = await statsResponse.Content.ReadFromJsonAsync<StatsRecord>(TestContext.Current.CancellationToken);

        Assert.NotNull(stats);
        Assert.Equal(StatsType.Weekly, stats.StatsType);
        Assert.True(stats.NewPosts >= 1, "NewPosts should be at least 1");
        Assert.True(stats.TotalPosts >= 1, "TotalPosts should be at least 1");
        // Capture initial values
        var initialNewPosts = stats.NewPosts;
        var initialComments = stats.ResultingComments;
        var initialReactions = stats.ResultingReactions;

        // --- 2. Add Comment ---
        var createCommentDto = new CreateCommentDto
        {
            PostId = postId,
            Content = "Stats Comment",
            AuthorId = authorId
        };
        var commentResponse = await _client.PostAsJsonAsync("/api/v1/comments", createCommentDto, TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.Created, commentResponse.StatusCode);
        var commentId = await commentResponse.Content.ReadFromJsonAsync<Guid>(TestContext.Current.CancellationToken);

        // Process Events
        await TestHelpers.ProcessPendingEventsAsync(_factory.Services, TestContext.Current.CancellationToken);

        // Verify Stats (ResultingComments should increase)
        statsResponse = await _client.GetAsync("/api/v1/stats/weekly", TestContext.Current.CancellationToken);
        stats = await statsResponse.Content.ReadFromJsonAsync<StatsRecord>(TestContext.Current.CancellationToken);
        Assert.NotNull(stats);
        Assert.Equal(initialComments + 1, stats.ResultingComments);

        // --- 3. Add Like (Reaction) ---
        var toggleLikeCommand = new ToggleLikeCommand(null, postId, null, "❤️", null); // Heart emoji
        var likeResponse = await _client.PostAsJsonAsync("/api/v1/reactions/toggle", toggleLikeCommand, TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.OK, likeResponse.StatusCode);

        // Process Events
        await TestHelpers.ProcessPendingEventsAsync(_factory.Services, TestContext.Current.CancellationToken);

        // Verify Stats (ResultingReactions increased, Breakdown contains emoji)
        statsResponse = await _client.GetAsync("/api/v1/stats/weekly", TestContext.Current.CancellationToken);
        stats = await statsResponse.Content.ReadFromJsonAsync<StatsRecord>(TestContext.Current.CancellationToken);
        Assert.NotNull(stats);
        Assert.Equal(initialReactions + 1, stats.ResultingReactions);

        var reactionStat = stats.ReactionBreakdown.FirstOrDefault(r => r.Emoji == "❤️");
        Assert.NotNull(reactionStat);
        Assert.True(reactionStat.Count >= 1);
    }
}