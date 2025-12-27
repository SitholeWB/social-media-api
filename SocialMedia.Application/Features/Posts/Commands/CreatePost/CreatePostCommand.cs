namespace SocialMedia.Application;

public record CreatePostCommand(CreatePostDto PostDto) : ICommand<Guid>;