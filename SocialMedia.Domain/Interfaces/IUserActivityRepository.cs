namespace SocialMedia.Domain;

public interface IUserActivityRepository : IRepository<UserActivity>
{
    Task<UserActivity?> GetByUserIdAsync(Guid userId, bool skipCache = false, CancellationToken cancellationToken = default);

    Task RefreshCacheAsync(Guid userId, CancellationToken cancellationToken = default);

    Task UpdateUserLastSeenAsync(Guid userId, CancellationToken cancellationToken = default);
}