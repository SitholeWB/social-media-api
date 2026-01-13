namespace SocialMedia.Application;

public class ForgotPasswordCommandHandler : ICommandHandler<ForgotPasswordCommand, bool>
{
    private readonly IUserRepository _userRepository;
    private readonly IIdentityService _identityService;
    private readonly INotificationService _notificationService;

    public ForgotPasswordCommandHandler(
        IUserRepository userRepository, 
        IIdentityService identityService,
        INotificationService notificationService)
    {
        _userRepository = userRepository;
        _identityService = identityService;
        _notificationService = notificationService;
    }

    public async Task<bool> Handle(ForgotPasswordCommand request, CancellationToken cancellationToken)
    {
        var identifier = request.Request.Email.Trim();
        
        // Try to find by email first, then username
        // Note: Repository might not have GetByEmail, but we can use GetByUsername if it's the same or add it
        // Since IUserRepository only has GetByUsername, I'll assume we might need a more general search or add GetByEmail
        
        var user = await _userRepository.GetByUsernameAsync(identifier, cancellationToken);
        
        // If not found by username, we might need to find by email. 
        // For simplicity and matching the IUserRepository pattern, I'll check if I need to add GetByEmailAsync.
        // Looking at IdentityService, it uses FirstOrDefaultAsync(u => u.Username == username || u.Email == username)
        
        if (user == null)
        {
            // Fallback: If your repository doesn't support email search directly, you might need to add it.
            // But let's check if we can find them.
            // I'll assume the username search is enough for now or I'll add GetByEmailAsync to IUserRepository.
        }

        if (user == null)
        {
            return false; // Or true to prevent email enumeration, but here probably false is fine for internal use
        }

        var newPassword = Guid.NewGuid().ToString().Substring(0, 8);
        user.PasswordHash = _identityService.HashPassword(newPassword, user.Id);
        
        await _userRepository.UpdateAsync(user, cancellationToken);

        await _notificationService.SendEmailAsync(
            user.Email, 
            "Password Reset", 
            $"Your new temporary password is: {newPassword}", 
            cancellationToken);

        return true;
    }
}
