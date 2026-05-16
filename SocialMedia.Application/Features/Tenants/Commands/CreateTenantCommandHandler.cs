namespace SocialMedia.Application;

public class CreateTenantCommandHandler : ICommandHandler<CreateTenantCommand, Guid>
{
    private readonly ITenantRepository _tenantRepository;

    public CreateTenantCommandHandler(ITenantRepository tenantRepository)
    {
        _tenantRepository = tenantRepository;
    }

    public async Task<Guid> HandleAsync(CreateTenantCommand command, CancellationToken cancellationToken)
    {
        var tenant = new Tenant
        {
            Name = command.Name,
            Description = command.Description,
            ThemeJson = command.ThemeJson
        };

        await _tenantRepository.AddAsync(tenant, cancellationToken);
        return tenant.Id;
    }
}
