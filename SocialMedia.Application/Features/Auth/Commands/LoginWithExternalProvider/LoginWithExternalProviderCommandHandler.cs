namespace SocialMedia.Application;

public class LoginWithExternalProviderCommandHandler : ICommandHandler<LoginWithExternalProviderCommand, AuthResponse>
{
    private readonly IIdentityService _identityService;

    public LoginWithExternalProviderCommandHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<AuthResponse> HandleAsync(LoginWithExternalProviderCommand request, CancellationToken cancellationToken)
    {
        return await _identityService.LoginWithExternalProviderAsync(request.Request, cancellationToken);
    }
}
