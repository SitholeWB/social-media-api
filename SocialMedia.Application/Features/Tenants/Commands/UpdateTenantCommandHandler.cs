namespace SocialMedia.Application;

public class UpdateTenantCommandHandler : ICommandHandler<UpdateTenantCommand, bool>
{
    private readonly ITenantRepository _tenantRepository;

    public UpdateTenantCommandHandler(ITenantRepository tenantRepository)
    {
        _tenantRepository = tenantRepository;
    }

    public async Task<bool> HandleAsync(UpdateTenantCommand command, CancellationToken cancellationToken)
    {
        var tenant = await _tenantRepository.GetByIdAsync(command.Id, cancellationToken);
        if (tenant == null)
        {
            throw new Exception("Tenant not found");
        }

        tenant.Name = command.Name;
        tenant.Description = command.Description;
        tenant.ThemeJson = command.ThemeJson;

        await _tenantRepository.UpdateAsync(tenant, cancellationToken);
        return true;
    }
}
