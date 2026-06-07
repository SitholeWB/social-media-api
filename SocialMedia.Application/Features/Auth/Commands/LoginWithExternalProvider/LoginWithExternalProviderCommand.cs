namespace SocialMedia.Application;

public record LoginWithExternalProviderCommand(ExternalLoginRequest Request) : ICommand<AuthResponse>;
