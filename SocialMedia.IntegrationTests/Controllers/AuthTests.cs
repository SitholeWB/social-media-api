using Microsoft.AspNetCore.Mvc.Testing;

namespace SocialMedia.IntegrationTests;

public class AuthTests : IClassFixture<WebApplicationFactory<Program>>
{
    private readonly WebApplicationFactory<Program> _factory;

    public AuthTests(WebApplicationFactory<Program> factory)
    {
        _factory = factory;
    }

    [Fact]
    public async Task Register_ShouldReturnAuthResponse_WhenRequestIsValid()
    {
        // Arrange
        var client = _factory.CreateClient();
        var request = new RegisterRequest("testuser", "test@example.com", "password123");

        // Act
        var response = await client.PostAsJsonAsync("/api/v1/auth/register", request);

        if (!response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            throw new Exception($"Request failed with status {response.StatusCode} and content: {content}");
        }
        response.EnsureSuccessStatusCode();
        var authResponse = await response.Content.ReadFromJsonAsync<AuthResponse>();
        Assert.NotNull(authResponse);
        Assert.Equal("testuser", authResponse.Username);
        Assert.Equal("test@example.com", authResponse.Email);
        Assert.False(string.IsNullOrEmpty(authResponse.Token));
    }

    [Fact]
    public async Task Login_ShouldReturnAuthResponse_WhenCredentialsAreValid()
    {
        // Arrange
        var client = _factory.CreateClient();
        var registerRequest = new RegisterRequest("loginuser", "login@example.com", "password123");
        await client.PostAsJsonAsync("/api/v1/auth/register", registerRequest);

        var loginRequest = new LoginRequest("loginuser", "password123");

        // Act
        var response = await client.PostAsJsonAsync("/api/v1/auth/login", loginRequest);

        if (!response.IsSuccessStatusCode)
        {
            var content = await response.Content.ReadAsStringAsync();
            throw new Exception($"Request failed with status {response.StatusCode} and content: {content}");
        }
        response.EnsureSuccessStatusCode();
        var authResponse = await response.Content.ReadFromJsonAsync<AuthResponse>();
        Assert.NotNull(authResponse);
        Assert.Equal("loginuser", authResponse.Username);
        Assert.False(string.IsNullOrEmpty(authResponse.Token));
    }

}
