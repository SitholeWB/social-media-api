namespace SocialMedia.Application;

public record CreateGroupCommand(string Name, string Description, GroupType Type, Guid CreatorId) : ICommand<Guid>;
