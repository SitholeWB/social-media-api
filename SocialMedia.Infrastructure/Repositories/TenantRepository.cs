namespace SocialMedia.Infrastructure;

public class TenantRepository : Repository<Tenant>, ITenantRepository
{
    public TenantRepository(SocialMediaDbContext context) : base(context)
    {
    }
}
