namespace SocialMedia.IntegrationTests;

public class BlockchainTests(IntegrationTestWebApplicationFactory factory) : BaseControllerTests(factory)
{
    public override async ValueTask InitializeAsync()
    {
        // Runs once before any tests in this class
        var uniqueId = Guid.NewGuid().ToString("N");
        var token = await RegisterAndLoginAsync($"likeuser_post_{uniqueId}@test.com", "password123", isAdmin: true);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    [Fact]
    public async Task VerifyChain_ShouldReturnTrue_Initially()
    {
        var response = await _client.GetAsync("/api/v1/polls/chain/verify", TestContext.Current.CancellationToken);
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<VerificationResult>(TestContext.Current.CancellationToken);
        Assert.True(result.IsValid);
    }

    [Fact]
    public async Task VerifyChain_ShouldReturnTrue_AfterVotes()
    {
        // Arrange: Create Poll
        var groupCommand = new CreateGroupCommand("BC Group", "Desc", GroupType.Everyone, Guid.NewGuid());
        var groupResponse = await _client.PostAsJsonAsync("/api/v1/groups", groupCommand, TestContext.Current.CancellationToken);
        var groupId = await groupResponse.Content.ReadFromJsonAsync<Guid>(TestContext.Current.CancellationToken);

        var createCommand = new CreatePollCommand(null, default, groupId)
        {
            Question = "Blockchain Test",
            Options = new List<string> { "A", "B" },
            ExpiresAt = DateTime.UtcNow.AddDays(1),
            CreatorId = Guid.NewGuid()
        };
        var createResponse = await _client.PostAsJsonAsync($"/api/v1/groups/{groupId}/polls", createCommand, TestContext.Current.CancellationToken);
        var pollId = await createResponse.Content.ReadFromJsonAsync<Guid>(TestContext.Current.CancellationToken);

        // Get Option
        var getResponse = await _client.GetAsync($"/api/v1/polls/{pollId}", TestContext.Current.CancellationToken);
        var poll = await getResponse.Content.ReadFromJsonAsync<PollDto>(TestContext.Current.CancellationToken);
        var optionId = poll.Options[0].Id;

        // Vote
        var voteCommand = new VoteOnPollCommand(default, default, default)
        {
            PollId = pollId,
            PollOptionId = optionId,
            UserId = Guid.NewGuid()
        };
        var voteResponse = await _client.PostAsJsonAsync($"/api/v1/polls/{pollId}/vote", voteCommand, TestContext.Current.CancellationToken);
        voteResponse.EnsureSuccessStatusCode();

        // Act: Verify Chain
        var verifyResponse = await _client.GetAsync("/api/v1/polls/chain/verify", TestContext.Current.CancellationToken);
        verifyResponse.EnsureSuccessStatusCode();
        var result = await verifyResponse.Content.ReadFromJsonAsync<VerificationResult>(TestContext.Current.CancellationToken);

        // Assert
        Assert.True(result.IsValid);
    }

    private class VerificationResult
    {
        public bool IsValid { get; set; }
    }
}