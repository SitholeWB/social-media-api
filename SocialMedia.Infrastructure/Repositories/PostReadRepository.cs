namespace SocialMedia.Infrastructure;

public class PostReadRepository : IPostReadRepository
{
    private readonly SocialMediaDbContext _readDbContext;
    private readonly ILogger<PostReadRepository> _logger;

    public PostReadRepository(ILogger<PostReadRepository> logger, SocialMediaDbContext writeDbContext)
    {
        _logger = logger;
        _readDbContext = writeDbContext;
    }

    public async Task AddAsync(PostReadModel post, CancellationToken cancellationToken = default)
    {
        await _readDbContext.PostReads.AddAsync(post, cancellationToken);
        await _readDbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(PostReadModel post, CancellationToken cancellationToken = default)
    {
        _readDbContext.PostReads.Update(post);
        await _readDbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<PostReadModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _readDbContext.PostReads.FindAsync(new object[] { id }, cancellationToken);
    }

    public async Task<bool> DeleteByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var post = await _readDbContext.PostReads.FindAsync(new object[] { id }, cancellationToken);
        if (post is null)
        {
            return false;
        }
        _readDbContext.PostReads.Remove(post);
        await _readDbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<long> GetTotalCountAsync(Guid groupId, CancellationToken token)
    {
        return await _readDbContext.PostReads.Where(x => x.GroupId == groupId).LongCountAsync(token);
    }
}