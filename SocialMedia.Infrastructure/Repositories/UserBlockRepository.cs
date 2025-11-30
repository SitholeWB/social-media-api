
namespace SocialMedia.Infrastructure;

public class UserBlockRepository : Repository<UserBlock>, IUserBlockRepository
{
    public UserBlockRepository(SocialMediaDbContext context) : base(context)
    {
    }

    public async Task<bool> ExistsAsync(Guid blockerId, Guid blockedUserId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.UserBlocks.AnyAsync(ub =>
            ub.BlockerId == blockerId && ub.BlockedUserId == blockedUserId, cancellationToken);
    }

}
