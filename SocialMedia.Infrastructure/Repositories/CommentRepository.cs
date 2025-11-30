
namespace SocialMedia.Infrastructure;

public class CommentRepository : Repository<Comment>, ICommentRepository
{
    public CommentRepository(SocialMediaDbContext dbContext) : base(dbContext)
    {
    }

    public override async Task<Comment?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Comments
            .Include(c => c.File)
            .FirstOrDefaultAsync(c => c.Id == id, cancellationToken);
    }
    public async Task<(List<Comment> Items, long TotalCount)> GetPagedByPostIdAsync(Guid postId, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Comments
            .Where(c => c.PostId == postId)
            .Include(c => c.Likes)
            .OrderBy(c => c.CreatedAt);

        var totalCount = await query.LongCountAsync(cancellationToken);
        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }
}
