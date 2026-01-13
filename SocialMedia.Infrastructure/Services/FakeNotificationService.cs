namespace SocialMedia.Infrastructure;

public class FakeNotificationService : INotificationService
{
    private readonly ILogger<FakeNotificationService> _logger;

    public FakeNotificationService(ILogger<FakeNotificationService> logger)
    {
        _logger = logger;
    }

    public Task SendEmailAsync(string to, string subject, string body, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Sending fake email to {To} with subject {Subject}", to, subject);
        return Task.CompletedTask;
    }

    public Task SendSmsAsync(string phoneNumber, string message, CancellationToken cancellationToken = default)
    {
        _logger.LogInformation("Sending fake SMS to {PhoneNumber} with message {Message}", phoneNumber, message);
        return Task.CompletedTask;
    }
}