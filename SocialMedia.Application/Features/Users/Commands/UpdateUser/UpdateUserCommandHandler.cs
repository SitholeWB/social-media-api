namespace SocialMedia.Application;

public class UpdateUserCommandHandler : ICommandHandler<UpdateUserCommand, bool>
{
    private readonly IUserRepository _userRepository;

    public UpdateUserCommandHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<bool> Handle(UpdateUserCommand request, CancellationToken cancellationToken)
    {
        var user = await _userRepository.GetByIdAsync(request.UserId, cancellationToken);
        if (user == null)
        {
            return false;
        }

        user.Names = request.Request.FirstName;
        user.Surname = !string.IsNullOrWhiteSpace(request.Request.LastName)
            ? request.Request.LastName
            : request.Request.Surname;
        user.Email = request.Request.Email;
        user.Avatar = request.Request.Avatar;

        await _userRepository.UpdateAsync(user, cancellationToken);
        return true;
    }
}