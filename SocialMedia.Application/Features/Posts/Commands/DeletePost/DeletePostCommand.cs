namespace SocialMedia.Application;

public record DeletePostCommand(Guid PostId, Guid? UserId) : ICommand<bool>;