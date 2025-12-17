namespace SocialMedia.Application;

public class GetUserByIdQueryHandler : IQueryHandler<GetUserByIdQuery, AuthResponse>
{
    private readonly IIdentityService _identityService;

    public GetUserByIdQueryHandler(IIdentityService identityService)
    {
        _identityService = identityService;
    }

    public async Task<AuthResponse> Handle(GetUserByIdQuery request, CancellationToken cancellationToken)
    {
        return await _identityService.GetUserByIdAsync(request.UserId, cancellationToken);
    }
}