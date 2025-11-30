namespace SocialMedia.Application;

public record UpdateCommentCommand(Guid Id, string Content) : ICommand<bool>;
