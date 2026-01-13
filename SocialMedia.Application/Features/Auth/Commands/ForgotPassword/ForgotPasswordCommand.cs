namespace SocialMedia.Application;

public record ForgotPasswordCommand(ForgotPasswordRequest Request) : ICommand<bool>;
