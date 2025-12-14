namespace SocialMedia.Infrastructure;

public class CommentReadRepository : ICommentReadRepository
{
    private readonly SocialMediaReadDbContext _context;

    public CommentReadRepository(SocialMediaReadDbContext context)
    {
        _context = context;
    }

    public async Task AddAsync(CommentReadModel comment, CancellationToken cancellationToken = default)
    {
        await _context.Comments.AddAsync(comment, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(CommentReadModel comment, CancellationToken cancellationToken = default)
    {
        _context.Comments.Update(comment);
        await _context.SaveChangesAsync(cancellationToken);
    }

    public async Task<CommentReadModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Comments.FindAsync(new object[] { id }, cancellationToken);
    }

    public async Task<bool> DeleteByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var comment = await _context.Comments.FindAsync(new object[] { id }, cancellationToken);
        if (comment is null)
        {
            return false;
        }
        _context.Comments.Remove(comment);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<List<CommentReadModel>> GetByPostIdAsync(Guid postId, int page, int pageSize, CancellationToken cancellationToken = default)
    {
        return await _context.Comments
            .Where(c => c.PostId == postId)
            .OrderByDescending(c => c.CreatedAt)
            .Skip((page - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);
    }
}