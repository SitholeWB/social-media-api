namespace SocialMedia.Application;

public record AdminBlockUserCommand(Guid UserId, bool IsBanned) : ICommand<bool>;