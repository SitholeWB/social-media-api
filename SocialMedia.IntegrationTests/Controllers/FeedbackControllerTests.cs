namespace SocialMedia.IntegrationTests;

using System.Net.Http.Json;

public class FeedbackControllerTests(IntegrationTestWebApplicationFactory factory) : BaseControllerTests(factory)
{
    [Fact]
    public async Task SubmitFeedback_ShouldReturnOk_WhenAuthorized()
    {
        // Arrange
        var uniqueId = Guid.NewGuid().ToString("N");
        var (token, userId) = await RegisterAndLoginAsync($"feedback_{uniqueId}", "password123");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var feedbackRequest = new SubmitFeedbackRequest
        {
            Content = "This is test feedback",
            UserId = userId
        };

        // Act
        var response = await _client.PostAsJsonAsync("/api/v1/feedback", feedbackRequest, TestContext.Current.CancellationToken);

        // Assert
        response.EnsureSuccessStatusCode();
        var result = await response.Content.ReadFromJsonAsync<bool>(TestContext.Current.CancellationToken);
        Assert.True(result);
    }
}