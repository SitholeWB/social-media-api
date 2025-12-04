namespace SocialMedia.Application;

public record UpdateGroupCommand(Guid GroupId, string Name, string Description, bool IsPublic, bool IsAutoAdd) : ICommand<bool>;
