using System.Text;

namespace SocialMedia.Application;

public class CreateDefaultTenantsCommandHandler : ICommandHandler<CreateDefaultTenantsCommand, string>
{
    private readonly ITenantRepository _tenantRepository;

    public CreateDefaultTenantsCommandHandler(ITenantRepository tenantRepository)
    {
        _tenantRepository = tenantRepository;
    }

    public async Task<string> HandleAsync(CreateDefaultTenantsCommand request, CancellationToken cancellationToken)
    {
        var sb = new StringBuilder();
        foreach (var defaultTenant in request.DefaultTenants)
        {
            var existingTenant = await _tenantRepository.GetByIdAsync(defaultTenant.Id, cancellationToken);
            if (existingTenant == null)
            {
                var tenant = new Tenant
                {
                    Id = defaultTenant.Id,
                    Name = defaultTenant.Name,
                    Description = defaultTenant.Description,
                    ThemeJson = defaultTenant.ThemeJson
                };
                await _tenantRepository.AddAsync(tenant, cancellationToken);
                sb.AppendLine($"Created tenant: {tenant.Name} (ID: {tenant.Id})");
            }
            else
            {
                sb.AppendLine($"Already exist tenant: {existingTenant.Name} (ID: {existingTenant.Id})");
            }
        }

        return sb.ToString();
    }
}
