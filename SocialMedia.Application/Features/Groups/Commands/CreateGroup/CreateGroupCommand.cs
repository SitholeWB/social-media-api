namespace SocialMedia.Application;

public record CreateGroupCommand(string Name, string Description, bool IsPublic, bool IsAutoAdd, GroupType Type, Guid CreatorId) : ICommand<Guid>;
