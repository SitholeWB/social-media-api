namespace SocialMedia.Application;

public class AdminBlockUserCommandHandler : ICommandHandler<AdminBlockUserCommand, bool>
{
    private readonly IUserRepository _userRepository;

    public AdminBlockUserCommandHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<bool> HandleAsync(AdminBlockUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);

        if (user == null)
        {
            throw new KeyNotFoundException($"User with ID {request.UserId} not found.");
        }

        user.IsBanned = request.IsBanned;
        await _userRepository.UpdateAsync(user, cancellationToken);

        return true;
    }
}