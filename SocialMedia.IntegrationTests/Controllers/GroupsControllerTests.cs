using System.Net.Http.Headers;

namespace SocialMedia.IntegrationTests;

public class GroupsControllerTests : IClassFixture<IntegrationTestWebApplicationFactory>
{
    private readonly IntegrationTestWebApplicationFactory _factory;
    private readonly HttpClient _client;

    public GroupsControllerTests(IntegrationTestWebApplicationFactory factory)
    {
        _factory = factory;
        _client = factory.CreateClient();
    }

    private async Task<string> RegisterAndLoginAsync(string username, string password)
    {
        var email = $"{username}@example.com";
        var registerRequest = new RegisterRequest(username, email, password);
        var registerResponse = await _client.PostAsJsonAsync("/api/v1/auth/register", registerRequest);
        registerResponse.EnsureSuccessStatusCode();

        var loginRequest = new LoginRequest(username, password);
        var loginResponse = await _client.PostAsJsonAsync("/api/v1/auth/login", loginRequest);
        loginResponse.EnsureSuccessStatusCode();
        var authResponse = await loginResponse.Content.ReadFromJsonAsync<AuthResponse>();
        return authResponse!.Token;
    }

    [Fact]
    public async Task CreateGroup_ShouldReturnBadRequest_WhenNameIsEmpty()
    {
        // Arrange
        var uniqueId = Guid.NewGuid().ToString("N");
        var token = await RegisterAndLoginAsync($"groupuser_bad_{uniqueId}", "password123");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var command = new CreateGroupCommand("", "Description", true, false);

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/groups", command);

        // Assert
        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task CreateGroup_ShouldReturnOk()
    {
        // Arrange
        var uniqueId = Guid.NewGuid().ToString("N");
        var token = await RegisterAndLoginAsync($"groupuser1_{uniqueId}", "password123");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var command = new CreateGroupCommand("Test Group", "Description", true, false);

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/groups", command);

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var groupId = await response.Content.ReadFromJsonAsync<Guid>();
        Assert.NotEqual(Guid.Empty, groupId);
    }

    [Fact]
    public async Task AddUserToGroup_ShouldReturnOk()
    {
        // Arrange
        var uniqueId = Guid.NewGuid().ToString("N");
        var token = await RegisterAndLoginAsync($"groupuser2_{uniqueId}", "password123");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var createCommand = new CreateGroupCommand("Test Group 2", "Description", true, false);
        var createResponse = await _client.PostAsJsonAsync("/api/v1/groups", createCommand);
        var groupId = await createResponse.Content.ReadFromJsonAsync<Guid>();
        var userId = Guid.NewGuid();

        // Act
        var response = await _client.PostAsync($"/api/v1/groups/{groupId}/users/{userId}", null);

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }

    [Fact]
    public async Task RemoveUserFromGroup_ShouldReturnOk()
    {
        // Arrange
        var uniqueId = Guid.NewGuid().ToString("N");
        var token = await RegisterAndLoginAsync($"groupuser3_{uniqueId}", "password123");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var createCommand = new CreateGroupCommand("Test Group 3", "Description", true, false);
        var createResponse = await _client.PostAsJsonAsync("/api/v1/groups", createCommand);
        var groupId = await createResponse.Content.ReadFromJsonAsync<Guid>();
        var userId = Guid.NewGuid();
        await _client.PostAsync($"/api/v1/groups/{groupId}/users/{userId}", null);

        // Act
        var response = await _client.DeleteAsync($"/api/v1/groups/{groupId}/users/{userId}");

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
    }
}
