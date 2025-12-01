namespace SocialMedia.IntegrationTests;

public class BlockchainTests : IClassFixture<IntegrationTestWebApplicationFactory>
{
    private readonly HttpClient _client;

    public BlockchainTests(IntegrationTestWebApplicationFactory factory)
    {
        _client = factory.CreateClient();
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
        var createCommand = new CreatePollCommand(null, default)
        {
            Question = "Blockchain Test",
            Options = new List<string> { "A", "B" },
            ExpiresAt = DateTime.UtcNow.AddDays(1),
            CreatorId = Guid.NewGuid()
        };
        var createResponse = await _client.PostAsJsonAsync("/api/v1/polls", createCommand, TestContext.Current.CancellationToken);
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
