namespace SocialMedia.Infrastructure;

public class UserActivityRepository : Repository<UserActivity>, IUserActivityRepository
{
    public UserActivityRepository(SocialMediaDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<UserActivity?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.UserActivities
            .FirstOrDefaultAsync(ua => ua.UserId == userId, cancellationToken);
    }
}
