namespace SocialMedia.Application;

public record RemoveUserFromGroupCommand(Guid GroupId, Guid UserId) : ICommand<bool>;