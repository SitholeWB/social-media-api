namespace SocialMedia.Application;

public record DeleteCommentCommand(Guid Id) : ICommand<bool>;
