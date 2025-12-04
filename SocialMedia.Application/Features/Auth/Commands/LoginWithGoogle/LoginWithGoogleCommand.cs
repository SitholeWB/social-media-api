using SocialMedia.Application;

namespace SocialMedia.Application;

public record LoginWithGoogleCommand(GoogleLoginRequest GoogleLoginRequest) : ICommand<AuthResponse>;
