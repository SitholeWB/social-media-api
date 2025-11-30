namespace SocialMedia.Application;

public record CreatePollCommand(DateTime? ExpiresAt, Guid CreatorId) : ICommand<Guid>
{
    public string Question { get; set; } = string.Empty;
    public List<string> Options { get; set; } = new List<string>();
}
