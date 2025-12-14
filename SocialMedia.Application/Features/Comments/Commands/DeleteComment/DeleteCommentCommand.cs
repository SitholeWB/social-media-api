namespace SocialMedia.Application;

public record DeleteCommentCommand(Guid Id, Guid UserId) : ICommand<bool>;