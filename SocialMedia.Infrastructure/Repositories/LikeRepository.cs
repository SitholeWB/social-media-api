
namespace SocialMedia.Infrastructure;

public class LikeRepository : Repository<Like>, ILikeRepository
{
    public LikeRepository(SocialMediaDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<Like?> GetByPostIdAndUserIdAsync(Guid postId, Guid userId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Likes
            .FirstOrDefaultAsync(l => l.PostId == postId && l.UserId == userId, cancellationToken);
    }

    public async Task<Like?> GetByCommentIdAndUserIdAsync(Guid commentId, Guid userId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Likes
            .FirstOrDefaultAsync(l => l.CommentId == commentId && l.UserId == userId, cancellationToken);
    }
}
