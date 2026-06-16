namespace SocialMedia.Application;

public record CreateDefaultTenantsCommand(ICollection<DefaultTenantDto> DefaultTenants) : ICommand<string>;
