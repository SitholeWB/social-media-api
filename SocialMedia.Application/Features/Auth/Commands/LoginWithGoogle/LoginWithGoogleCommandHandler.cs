namespace SocialMedia.Application;

public class LoginWithGoogleCommandHandler : ICommandHandler<LoginWithGoogleCommand, AuthResponse>
{
    private readonly IIdentityService _identityService;

    public LoginWithGoogleCommandHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<AuthResponse> Handle(LoginWithGoogleCommand request, CancellationToken cancellationToken)
    {
        return await _identityService.LoginWithGoogleAsync(request.GoogleLoginRequest, cancellationToken);
    }
}