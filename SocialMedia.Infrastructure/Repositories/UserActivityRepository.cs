namespace SocialMedia.Infrastructure;

public class UserActivityRepository : Repository<UserActivity>, IUserActivityRepository
{
    private readonly IDistributedCache _cache;

    public UserActivityRepository(SocialMediaDbContext dbContext, IDistributedCache cache) : base(dbContext)
    {
        _cache = cache;
    }

    public async Task RefreshCacheAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"user_activity_{userId}";
        await _cache.RemoveAsync(cacheKey, cancellationToken);
        await GetByUserIdAsync(userId, false, cancellationToken);
    }

    public async Task<UserActivity?> GetByUserIdAsync(Guid userId, bool skipCache = false, CancellationToken cancellationToken = default)
    {
        var cacheKey = $"user_activity_{userId}";
        if (!skipCache)
        {
            // Try get from cache
            var cachedData = await _cache.GetStringAsync(cacheKey, cancellationToken);
            if (!string.IsNullOrEmpty(cachedData))
            {
                return JsonSerializer.Deserialize<UserActivity>(cachedData);
            }
        }
        // Get from DB
        var userActivity = await _dbContext.UserActivities
            .FirstOrDefaultAsync(ua => ua.UserId == userId, cancellationToken);

        // Set to cache
        if (userActivity != null)
        {
            var options = new DistributedCacheEntryOptions
            {
                AbsoluteExpirationRelativeToNow = TimeSpan.FromHours(1),
            };
            var serialized = JsonSerializer.Serialize(userActivity);
            await _cache.SetStringAsync(cacheKey, serialized, options, cancellationToken);
        }
        return userActivity;
    }

    public async Task UpdateUserLastSeenAsync(Guid userId, CancellationToken cancellationToken = default)
    {
        var userActivity = await _dbContext.UserActivities.FirstOrDefaultAsync(ua => ua.UserId == userId, cancellationToken);
        if (userActivity != null)
        {
            userActivity.LastSeenAt = DateTimeOffset.UtcNow;
            await _dbContext.SaveChangesAsync(cancellationToken);
        }
    }
}