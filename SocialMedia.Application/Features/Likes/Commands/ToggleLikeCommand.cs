namespace SocialMedia.Application;

public record ToggleLikeCommand(Guid? UserId, Guid? PostId, Guid? CommentId, string Emoji, string? Username) : ICommand<bool>
{
    public string? Username { get; set; } = Username ?? "Unknown";
    public Guid? UserId { get; set; } = UserId ?? Guid.Empty;
}