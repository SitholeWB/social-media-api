namespace SocialMedia.Application;

public class EmailNotificationHandler : IEventHandler<PostCreatedEvent>
{
    public Task HandleAsync(PostCreatedEvent @event, CancellationToken cancellationToken)
    {
        // Simulate sending email
        Console.WriteLine($"[EmailNotificationHandler] Sending email for new post {@event.Post.Id} by author {@event.Post.AuthorId}");
        return Task.CompletedTask;
    }
}