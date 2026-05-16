namespace SocialMedia.Application;

public record CreateTenantCommand(string Name, string? Description, string? ThemeJson) : ICommand<Guid>;
