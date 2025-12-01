

namespace SocialMedia.IntegrationTests;

public class PollsControllerTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly HttpClient _client;

    public PollsControllerTests(WebApplicationFactory<Program> factory)
    {
        _client = factory.CreateClient();
    }

    [Fact]
    public async Task CreatePoll_ShouldReturnCreated()
    {
        try
        {
            // Arrange
            var command = new CreatePollCommand(null, default)
            {
                Question = "What is your favorite color?",
                Options = new List<string> { "Red", "Blue", "Green" },
                ExpiresAt = DateTime.UtcNow.AddDays(1),
                CreatorId = Guid.NewGuid()
            };

            // Act
            var response = await _client.PostAsJsonAsync("/api/v1/polls", command);

            // Assert
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync();
                Console.WriteLine($"CreatePoll failed: {response.StatusCode} - {error}");
            }
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.Created, response.StatusCode);
            var pollDto = await response.Content.ReadFromJsonAsync<Guid>();
            Assert.NotEqual(Guid.Empty, pollDto);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"CreatePoll Exception: {ex}");
            throw;
        }
    }

    [Fact]
    public async Task Vote_ShouldReturnOk_WhenVoteIsValid()
    {
        try
        {
            // Arrange
            var createCommand = new CreatePollCommand(null, default)
            {
                Question = "Vote Test",
                Options = new List<string> { "Option 1", "Option 2" },
                ExpiresAt = DateTime.UtcNow.AddDays(1),
                CreatorId = Guid.NewGuid()
            };
            var createResponse = await _client.PostAsJsonAsync("/api/v1/polls", createCommand);
            createResponse.EnsureSuccessStatusCode();
            var pollId = await createResponse.Content.ReadFromJsonAsync<Guid>();

            // Get the poll to find option IDs
            var getResponse = await _client.GetAsync($"/api/v1/polls/{pollId}");
            if (!getResponse.IsSuccessStatusCode)
            {
                var error = await getResponse.Content.ReadAsStringAsync();
                Console.WriteLine($"GetPoll failed: {getResponse.StatusCode} - {error}");
            }
            getResponse.EnsureSuccessStatusCode();
            var poll = await getResponse.Content.ReadFromJsonAsync<PollDto>();
            Assert.NotNull(poll);
            var optionId = poll!.Options[0].Id;

            var voteCommand = new VoteOnPollCommand(default, default, default)
            {
                PollId = pollId,
                PollOptionId = optionId,
                UserId = Guid.NewGuid()
            };

            // Act
            var voteResponse = await _client.PostAsJsonAsync($"/api/v1/polls/{pollId}/vote", voteCommand);

            // Assert
            if (!voteResponse.IsSuccessStatusCode)
            {
                var error = await voteResponse.Content.ReadAsStringAsync();
                Console.WriteLine($"Vote failed: {voteResponse.StatusCode} - {error}");
            }
            voteResponse.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, voteResponse.StatusCode);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Vote Exception: {ex}");
            throw;
        }
    }

    [Fact]
    public async Task Vote_ShouldReturnBadRequest_WhenUserVotesTwice()
    {
        try
        {
            // Arrange
            var createCommand = new CreatePollCommand(null, default)
            {
                Question = "Double Vote Test",
                Options = new List<string> { "Yes", "No" },
                ExpiresAt = DateTime.UtcNow.AddDays(1),
                CreatorId = Guid.NewGuid()
            };
            var createResponse = await _client.PostAsJsonAsync("/api/v1/polls", createCommand);
            var pollId = await createResponse.Content.ReadFromJsonAsync<Guid>();

            var getResponse = await _client.GetAsync($"/api/v1/polls/{pollId}");
            var poll = await getResponse.Content.ReadFromJsonAsync<PollDto>();
            Assert.NotNull(poll);
            var optionId = poll!.Options[0].Id;
            var userId = Guid.NewGuid();

            var voteCommand = new VoteOnPollCommand(default, default, default)
            {
                PollId = pollId,
                PollOptionId = optionId,
                UserId = userId
            };

            // Act
            await _client.PostAsJsonAsync($"/api/v1/polls/{pollId}/vote", voteCommand);
            var secondVoteResponse = await _client.PostAsJsonAsync($"/api/v1/polls/{pollId}/vote", voteCommand);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, secondVoteResponse.StatusCode);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"DoubleVote Exception: {ex}");
            throw;
        }
    }
}
