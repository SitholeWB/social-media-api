
namespace SocialMedia.Application;

public record BlockUserCommand(Guid BlockerId, Guid BlockedUserId) : ICommand<bool>;
