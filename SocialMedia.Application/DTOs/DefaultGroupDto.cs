namespace SocialMedia.Application;

public record DefaultGroupDto(Guid Id, string Name, bool IsPublic, bool IsAutoAdd, string Description = "");