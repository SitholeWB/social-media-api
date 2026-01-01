namespace SocialMedia.IntegrationTests;

public class PollsControllerTests(IntegrationTestWebApplicationFactory factory) : BaseControllerTests(factory)
{
    public override async ValueTask InitializeAsync()
    {
        // Runs once before any tests in this class
        var uniqueId = Guid.NewGuid().ToString("N");
        var token = await RegisterAndLoginAsync($"likeuser_post_{uniqueId}@test.com", "password123", isAdmin: true);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
    }

    [Fact]
    public async Task CreatePoll_ShouldReturnCreated()
    {
        try
        {
            // Create a group first
            var groupCommand = new CreateGroupCommand("Double Vote Group", "Desc", GroupType.Everyone, Guid.NewGuid());
            var groupResponse = await _client.PostAsJsonAsync("/api/v1/groups", groupCommand, TestContext.Current.CancellationToken);
            var groupId = await groupResponse.Content.ReadFromJsonAsync<Guid>(TestContext.Current.CancellationToken);

            // Arrange
            var command = new CreatePollCommand(null, default, groupId)
            {
                Question = "What is your favorite color?",
                Options = new List<string> { "Red", "Blue", "Green" },
                ExpiresAt = DateTime.UtcNow.AddDays(1),
                CreatorId = Guid.NewGuid()
            };

            // Act
            var response = await _client.PostAsJsonAsync($"/api/v1/groups/{groupId}/polls", command, TestContext.Current.CancellationToken);

            // Assert
            if (!response.IsSuccessStatusCode)
            {
                var error = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
                Console.WriteLine($"CreatePoll failed: {response.StatusCode} - {error}");
            }
            response.EnsureSuccessStatusCode();
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var pollDto = await response.Content.ReadFromJsonAsync<Guid>(TestContext.Current.CancellationToken);
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
            // Create a group first
            var groupCommand = new CreateGroupCommand("Vote Test Group", "Desc", GroupType.Everyone, Guid.NewGuid());
            var groupResponse = await _client.PostAsJsonAsync("/api/v1/groups", groupCommand, TestContext.Current.CancellationToken);
            var groupId = await groupResponse.Content.ReadFromJsonAsync<Guid>(TestContext.Current.CancellationToken);

            // Arrange
            var createCommand = new CreatePollCommand(null, default, groupId)
            {
                Question = "Vote Test",
                Options = new List<string> { "Option 1", "Option 2" },
                ExpiresAt = DateTime.UtcNow.AddDays(1),
                CreatorId = Guid.NewGuid()
            };
            var createResponse = await _client.PostAsJsonAsync($"/api/v1/groups/{groupId}/polls", createCommand, TestContext.Current.CancellationToken);
            createResponse.EnsureSuccessStatusCode();
            var pollId = await createResponse.Content.ReadFromJsonAsync<Guid>(TestContext.Current.CancellationToken);

            // Get the poll to find option IDs
            var getResponse = await _client.GetAsync($"/api/v1/polls/{pollId}", TestContext.Current.CancellationToken);
            if (!getResponse.IsSuccessStatusCode)
            {
                var error = await getResponse.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
                Console.WriteLine($"GetPoll failed: {getResponse.StatusCode} - {error}");
            }
            getResponse.EnsureSuccessStatusCode();
            var poll = await getResponse.Content.ReadFromJsonAsync<PollDto>(TestContext.Current.CancellationToken);
            Assert.NotNull(poll);
            var optionId = poll!.Options[0].Id;

            var voteCommand = new VoteOnPollCommand(default, default, default)
            {
                PollId = pollId,
                PollOptionId = optionId,
                UserId = Guid.NewGuid()
            };

            // Act
            var voteResponse = await _client.PostAsJsonAsync($"/api/v1/polls/{pollId}/vote", voteCommand, TestContext.Current.CancellationToken);

            // Assert
            if (!voteResponse.IsSuccessStatusCode)
            {
                var error = await voteResponse.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
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
            // Create a group first
            var groupCommand = new CreateGroupCommand("Double Vote Group", "Desc", GroupType.Everyone, Guid.NewGuid());
            var groupResponse = await _client.PostAsJsonAsync("/api/v1/groups", groupCommand, TestContext.Current.CancellationToken);
            var groupId = await groupResponse.Content.ReadFromJsonAsync<Guid>(TestContext.Current.CancellationToken);

            // Arrange
            var createCommand = new CreatePollCommand(null, default, groupId)
            {
                Question = "Double Vote Test",
                Options = new List<string> { "Yes", "No" },
                ExpiresAt = DateTime.UtcNow.AddDays(1),
                CreatorId = Guid.NewGuid()
            };
            var createResponse = await _client.PostAsJsonAsync($"/api/v1/groups/{groupId}/polls", createCommand, TestContext.Current.CancellationToken);
            var pollId = await createResponse.Content.ReadFromJsonAsync<Guid>(TestContext.Current.CancellationToken);

            var getResponse = await _client.GetAsync($"/api/v1/polls/{pollId}", TestContext.Current.CancellationToken);
            var poll = await getResponse.Content.ReadFromJsonAsync<PollDto>(TestContext.Current.CancellationToken);
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
            await _client.PostAsJsonAsync($"/api/v1/polls/{pollId}/vote", voteCommand, TestContext.Current.CancellationToken);
            var secondVoteResponse = await _client.PostAsJsonAsync($"/api/v1/polls/{pollId}/vote", voteCommand, TestContext.Current.CancellationToken);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, secondVoteResponse.StatusCode);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"DoubleVote Exception: {ex}");
            throw;
        }
    }

    [Fact]
    public async Task GetPoll_ShouldReturnNotFound_WhenPollDoesNotExist()
    {
        var pollId = Guid.NewGuid();
        var response = await _client.GetAsync($"/api/v1/polls/{pollId}", TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task UpdatePoll_ShouldReturnNotFound_WhenPollDoesNotExist()
    {
        // Arrange
        var pollId = Guid.NewGuid();
        var command = new UpdatePollCommand(pollId, "Updated Question", true, DateTime.UtcNow.AddDays(1), false, Guid.NewGuid());

        // Act
        var response = await _client.PutAsJsonAsync($"/api/v1/polls/{pollId}", command, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DeletePoll_ShouldReturnNotFound_WhenPollDoesNotExist()
    {
        // Arrange
        var pollId = Guid.NewGuid();

        // Act
        var response = await _client.DeleteAsync($"/api/v1/polls/{pollId}", TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task UpdatePoll_ShouldReturnNoContent_WhenPollExists()
    {
        // Create a group first
        var groupCommand = new CreateGroupCommand("Update Poll Group", "Desc", GroupType.Everyone, Guid.NewGuid());
        var groupResponse = await _client.PostAsJsonAsync("/api/v1/groups", groupCommand, TestContext.Current.CancellationToken);
        var groupId = await groupResponse.Content.ReadFromJsonAsync<Guid>(TestContext.Current.CancellationToken);

        // Arrange
        var createCommand = new CreatePollCommand(null, default, groupId)
        {
            Question = "Poll to Update",
            Options = new List<string> { "A", "B" },
            ExpiresAt = DateTime.UtcNow.AddDays(1),
            CreatorId = Guid.NewGuid()
        };
        var createResponse = await _client.PostAsJsonAsync($"/api/v1/groups/{groupId}/polls", createCommand, TestContext.Current.CancellationToken);
        var pollId = await createResponse.Content.ReadFromJsonAsync<Guid>(TestContext.Current.CancellationToken);

        var updateCommand = new UpdatePollCommand(pollId, "Updated Question", true, DateTime.UtcNow.AddDays(2), false, groupId);

        // Act
        var response = await _client.PutAsJsonAsync($"/api/v1/polls/{pollId}", updateCommand, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }

    [Fact]
    public async Task DeletePoll_ShouldReturnNoContent_WhenPollExists()
    {
        // Create a group first
        var groupCommand = new CreateGroupCommand("Delete Poll Group", "Desc", GroupType.Everyone, Guid.NewGuid());
        var groupResponse = await _client.PostAsJsonAsync("/api/v1/groups", groupCommand, TestContext.Current.CancellationToken);
        var groupId = await groupResponse.Content.ReadFromJsonAsync<Guid>(TestContext.Current.CancellationToken);

        // Arrange
        var createCommand = new CreatePollCommand(null, default, groupId)
        {
            Question = "Poll to Delete",
            Options = new List<string> { "A", "B" },
            ExpiresAt = DateTime.UtcNow.AddDays(1),
            CreatorId = Guid.NewGuid()
        };
        var createResponse = await _client.PostAsJsonAsync($"/api/v1/groups/{groupId}/polls", createCommand, TestContext.Current.CancellationToken);
        var pollId = await createResponse.Content.ReadFromJsonAsync<Guid>(TestContext.Current.CancellationToken);

        // Act
        var response = await _client.DeleteAsync($"/api/v1/polls/{pollId}", TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }
}