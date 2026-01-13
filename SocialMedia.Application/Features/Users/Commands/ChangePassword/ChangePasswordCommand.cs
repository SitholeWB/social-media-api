namespace SocialMedia.Application;

public record ChangePasswordCommand(Guid UserId, ChangePasswordRequest Request) : ICommand<bool>;
