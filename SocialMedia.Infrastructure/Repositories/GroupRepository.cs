
namespace SocialMedia.Infrastructure;

public class GroupRepository : Repository<Group>, IGroupRepository
{
    public GroupRepository(SocialMediaDbContext context) : base(context)
    {
    }

}
