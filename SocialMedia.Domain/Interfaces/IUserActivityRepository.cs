namespace SocialMedia.Domain;

public interface IUserActivityRepository : IRepository<UserActivity>
{
    Task<UserActivity?> GetByUserIdAsync(Guid userId, CancellationToken cancellationToken = default);
}
