namespace SocialMedia.Application;

public record ToggleLikeCommand(Guid UserId, Guid? PostId, Guid? CommentId, string Emoji) : ICommand<bool>;
