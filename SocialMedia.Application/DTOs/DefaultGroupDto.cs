namespace SocialMedia.Application;

public record DefaultGroupDto(Guid Id, string Name, GroupType Type, string Description = "");