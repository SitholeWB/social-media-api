
namespace SocialMedia.Infrastructure;

public class PostRepository : Repository<Post>, IPostRepository
{
    public PostRepository(SocialMediaDbContext dbContext) : base(dbContext)
    {
    }

    public override async Task<Post?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Posts
            .Include(p => p.File)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public override async Task<IReadOnlyList<Post>> GetAllAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Posts
            .Include(p => p.File)
            .ToListAsync(cancellationToken);
    }
    public async Task<(List<Post> Items, long TotalCount)> GetPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Posts
            .Include(p => p.File)
            .Include(p => p.Comments)
            .Include(p => p.Likes)
            .OrderByDescending(p => p.CreatedAt);

        var totalCount = await query.LongCountAsync(cancellationToken);
        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }
}
