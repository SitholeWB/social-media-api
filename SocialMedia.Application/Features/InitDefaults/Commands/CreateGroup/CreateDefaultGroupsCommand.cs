namespace SocialMedia.Application;

public record CreateDefaultGroupsCommand(ICollection<DefaultGroupDto> DefaultGroups) : ICommand<string>;