namespace SocialMedia.Application;

public record LoginCommand(LoginRequest LoginRequest) : ICommand<AuthResponse>;
