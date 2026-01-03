namespace SocialMedia.Infrastructure;

public class PostReadRepository : IPostReadRepository
{
    private readonly SocialMediaReadDbContext _readDbContext;
    private readonly SocialMediaDbContext _writeDbContext;
    private readonly ILogger<PostReadRepository> _logger;

    public PostReadRepository(SocialMediaReadDbContext readDbContext, ILogger<PostReadRepository> logger, SocialMediaDbContext writeDbContext)
    {
        _readDbContext = readDbContext;
        _logger = logger;
        _writeDbContext = writeDbContext;
    }

    public async Task AddAsync(PostReadModel post, CancellationToken cancellationToken = default)
    {
        await _readDbContext.Posts.AddAsync(post, cancellationToken);
        await _readDbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(PostReadModel post, CancellationToken cancellationToken = default)
    {
        _readDbContext.Posts.Update(post);
        await _readDbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<PostReadModel?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _readDbContext.Posts.FindAsync(new object[] { id }, cancellationToken);
    }

    public async Task<bool> DeleteByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var post = await _readDbContext.Posts.FindAsync(new object[] { id }, cancellationToken);
        if (post is null)
        {
            return false;
        }
        _readDbContext.Posts.Remove(post);
        await _readDbContext.SaveChangesAsync(cancellationToken);
        return true;
    }

    public async Task<long> GetTotalCountAsync(Guid groupId, CancellationToken token)
    {
        return await _readDbContext.Posts.Where(x => x.GroupId == groupId).LongCountAsync(token);
    }
}