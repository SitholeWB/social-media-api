namespace SocialMedia.Application;

public class GetTenantsQueryHandler : IQueryHandler<GetTenantsQuery, List<TenantDto>>
{
    private readonly ITenantRepository _tenantRepository;

    public GetTenantsQueryHandler(ITenantRepository tenantRepository)
    {
        _tenantRepository = tenantRepository;
    }

    public async Task<List<TenantDto>> HandleAsync(GetTenantsQuery query, CancellationToken cancellationToken)
    {
        var tenants = await _tenantRepository.GetAllAsync(cancellationToken);
        return tenants.Select(t => new TenantDto
        {
            Id = t.Id,
            Name = t.Name,
            Description = t.Description,
            ThemeJson = t.ThemeJson,
            CreatedAt = t.CreatedAt
        }).ToList();
    }
}
