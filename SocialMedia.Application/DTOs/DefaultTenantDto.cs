namespace SocialMedia.Application;

public record DefaultTenantDto(Guid Id, string Name, string? Description = null, string? ThemeJson = null);
