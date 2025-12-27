namespace SocialMedia.Application;

public record UpdateGroupCommand(Guid GroupId, string Name, string Description, GroupType Type) : ICommand<bool>;