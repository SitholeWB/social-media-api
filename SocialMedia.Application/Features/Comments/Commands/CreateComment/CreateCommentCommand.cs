namespace SocialMedia.Application;

public record CreateCommentCommand(CreateCommentDto CreateCommentDto) : ICommand<Guid>;
