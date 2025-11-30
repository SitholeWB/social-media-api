namespace SocialMedia.Application;

public record CreateGroupCommand(string Name, string Description, bool IsPublic, bool IsAutoAdd) : ICommand<Guid>;
