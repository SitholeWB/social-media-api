namespace SocialMedia.Application;

public class ExchangeTenantCommandHandler : ICommandHandler<ExchangeTenantCommand, AuthResponse>
{
    private readonly IIdentityService _identityService;

    public ExchangeTenantCommandHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<AuthResponse> HandleAsync(ExchangeTenantCommand request, CancellationToken cancellationToken)
    {
        return await _identityService.ExchangeTenantAsync(request.UserId, request.NewTenantId, cancellationToken);
    }
}
