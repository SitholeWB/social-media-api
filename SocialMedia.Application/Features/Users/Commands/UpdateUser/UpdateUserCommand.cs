namespace SocialMedia.Application;

public record UpdateUserCommand(Guid UserId, UpdateUserRequest Request) : ICommand<bool>;
