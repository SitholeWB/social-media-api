namespace SocialMedia.Domain;

public interface ILikeRepository : IRepository<Like>
{
    Task<Like?> GetByPostIdAndUserIdAsync(Guid postId, Guid userId, CancellationToken cancellationToken = default);

    Task<Like?> GetByCommentIdAndUserIdAsync(Guid commentId, Guid userId, CancellationToken cancellationToken = default);
}