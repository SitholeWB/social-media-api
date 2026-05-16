namespace SocialMedia.Application;

public record GetTenantsQuery() : IQuery<List<TenantDto>>;
