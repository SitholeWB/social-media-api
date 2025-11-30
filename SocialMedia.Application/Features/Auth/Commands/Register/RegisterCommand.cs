namespace SocialMedia.Application;

public record RegisterCommand(RegisterRequest RegisterRequest) : ICommand<AuthResponse>;
