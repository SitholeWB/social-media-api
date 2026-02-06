namespace SocialMedia.Application;

public class ChangePasswordCommandHandler : ICommandHandler<ChangePasswordCommand, bool>
{
    private readonly IUserRepository _userRepository;
    private readonly IIdentityService _identityService;

    public ChangePasswordCommandHandler(IUserRepository userRepository, IIdentityService identityService)
    {
        _userRepository = userRepository;
        _identityService = identityService;
    }

    public async Task<bool> HandleAsync(ChangePasswordCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user == null)
        {
            return false;
        }

        if (request.Request.NewPassword != request.Request.ConfirmPassword)
        {
            throw new ArgumentException("Passwords do not match");
        }

        if (!_identityService.VerifyPassword(request.Request.OldPassword, user.PasswordHash, user.Id))
        {
            throw new ArgumentException("Invalid old password");
        }

        user.PasswordHash = _identityService.HashPassword(request.Request.NewPassword, user.Id);
        await _userRepository.UpdateAsync(user, cancellationToken);

        return true;
    }
}
