namespace SocialMedia.Application;

public class DeleteTenantCommandHandler : ICommandHandler<DeleteTenantCommand, bool>
{
    private readonly ITenantRepository _tenantRepository;

    public DeleteTenantCommandHandler(ITenantRepository tenantRepository)
    {
        _tenantRepository = tenantRepository;
    }

    public async Task<bool> HandleAsync(DeleteTenantCommand command, CancellationToken cancellationToken)
    {
        var tenant = await _tenantRepository.GetByIdAsync(command.Id, cancellationToken);
        if (tenant == null)
        {
            throw new Exception("Tenant not found");
        }

        await _tenantRepository.DeleteAsync(tenant, cancellationToken);
        return true;
    }
}
