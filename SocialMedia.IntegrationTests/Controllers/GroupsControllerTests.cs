namespace SocialMedia.IntegrationTests;

public class GroupsControllerTests(IntegrationTestWebApplicationFactory factory) : BaseControllerTests(factory)
{
    [Fact]
    public async Task CreateGroup_ShouldReturnBadRequest_WhenNameIsEmpty()
    {
        // Arrange
        var command = new CreateGroupCommand("", "Description", true, false);

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/groups", command, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateGroup_ShouldReturnOk()
    {
        // Arrange
        var command = new CreateGroupCommand("Test Group", "Description", true, false);

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/groups", command, TestContext.Current.CancellationToken);

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var groupId = await response.Content.ReadFromJsonAsync<Guid>(TestContext.Current.CancellationToken);
        Assert.NotEqual(Guid.Empty, groupId);
    }

    [Fact]
    public async Task AddUserToGroup_ShouldReturnOk()
    {
        // Arrange
        var createCommand = new CreateGroupCommand("Test Group 2", "Description", true, false);
        var createResponse = await _client.PostAsJsonAsync("/api/v1/groups", createCommand, TestContext.Current.CancellationToken);
        var groupId = await createResponse.Content.ReadFromJsonAsync<Guid>(TestContext.Current.CancellationToken);
        var userId = Guid.NewGuid();

        // Act
        var response = await _client.PostAsync($"/api/v1/groups/{groupId}/users/{userId}", null, TestContext.Current.CancellationToken);

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task RemoveUserFromGroup_ShouldReturnOk()
    {
        // Arrange
        var createCommand = new CreateGroupCommand("Test Group 3", "Description", true, false);
        var createResponse = await _client.PostAsJsonAsync("/api/v1/groups", createCommand, TestContext.Current.CancellationToken);
        var groupId = await createResponse.Content.ReadFromJsonAsync<Guid>(TestContext.Current.CancellationToken);
        var userId = Guid.NewGuid();
        await _client.PostAsync($"/api/v1/groups/{groupId}/users/{userId}", null, TestContext.Current.CancellationToken);

        // Act
        var response = await _client.DeleteAsync($"/api/v1/groups/{groupId}/users/{userId}", TestContext.Current.CancellationToken);

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task UpdateGroup_ShouldReturnNotFound_WhenGroupDoesNotExist()
    {
        // Arrange
        var groupId = Guid.NewGuid();
        var command = new UpdateGroupCommand(groupId, "Updated Name", "Updated Desc", false, true);

        // Act
        var response = await _client.PutAsJsonAsync($"/api/v1/groups/{groupId}", command, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task DeleteGroup_ShouldReturnNotFound_WhenGroupDoesNotExist()
    {
        // Arrange
        var groupId = Guid.NewGuid();

        // Act
        var response = await _client.DeleteAsync($"/api/v1/groups/{groupId}", TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
    }

    [Fact]
    public async Task UpdateGroup_ShouldReturnNoContent_WhenGroupExists()
    {
        // Arrange
        var createCommand = new CreateGroupCommand("Group to Update", "Desc", true, false);
        var createResponse = await _client.PostAsJsonAsync("/api/v1/groups", createCommand, TestContext.Current.CancellationToken);
        var groupId = await createResponse.Content.ReadFromJsonAsync<Guid>(TestContext.Current.CancellationToken);

        var updateCommand = new UpdateGroupCommand(groupId, "Updated Name", "Updated Desc", false, true);

        // Act
        var response = await _client.PutAsJsonAsync($"/api/v1/groups/{groupId}", updateCommand, TestContext.Current.CancellationToken);

        // Assert
        Assert.Equal(HttpStatusCode.NoContent, response.StatusCode);
    }
}