
namespace SocialMedia.Infrastructure;

public class GroupMemberRepository : Repository<GroupMember>, IGroupMemberRepository
{
    public GroupMemberRepository(SocialMediaDbContext context) : base(context)
    {
    }

    public async Task<bool> ExistsAsync(Guid groupId, Guid userId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.GroupMembers.AnyAsync(gm =>
            gm.GroupId == groupId && gm.UserId == userId, cancellationToken);
    }

    public async Task<GroupMember?> GetByGroupAndUserAsync(Guid groupId, Guid userId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.GroupMembers.FirstOrDefaultAsync(gm =>
            gm.GroupId == groupId && gm.UserId == userId, cancellationToken);
    }

}
