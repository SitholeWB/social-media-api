namespace SocialMedia.Domain;

public interface IUserBlockRepository : IRepository<UserBlock>
{
    Task<bool> ExistsAsync(Guid blockerId, Guid blockedUserId, CancellationToken cancellationToken = default);
}
