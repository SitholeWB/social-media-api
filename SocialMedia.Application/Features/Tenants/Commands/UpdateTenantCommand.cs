namespace SocialMedia.Application;

public record UpdateTenantCommand(Guid Id, string Name, string? Description, string? ThemeJson) : ICommand<bool>;
