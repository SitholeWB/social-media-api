namespace SocialMedia.Application;

public interface IIdentityService
{
    Task<AuthResponse> GetUserByIdAsync(Guid userId, CancellationToken cancellationToken = default);

    Task<AuthResponse> LoginAsync(LoginRequest request, CancellationToken cancellationToken = default);

    Task<AuthResponse> LoginWithGoogleAsync(GoogleLoginRequest request, CancellationToken cancellationToken = default);

    Task<AuthResponse> RegisterAsync(RegisterRequest request, CancellationToken cancellationToken = default);
}