
namespace SocialMedia.Infrastructure;

public class PollRepository : IPollRepository
{
    private readonly SocialMediaDbContext _dbContext;

    public PollRepository(SocialMediaDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task<Poll?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Polls
            .Include(p => p.Options)
            .ThenInclude(o => o.Votes)
            .FirstOrDefaultAsync(p => p.Id == id, cancellationToken);
    }

    public async Task AddAsync(Poll poll, CancellationToken cancellationToken = default)
    {
        await _dbContext.Polls.AddAsync(poll, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(Poll poll, CancellationToken cancellationToken = default)
    {
        _dbContext.Polls.Update(poll);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<bool> HasUserVotedAsync(Guid pollId, Guid userId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Votes
            .Include(v => v.PollOption)
            .AnyAsync(v => v.PollOption != null && v.PollOption.PollId == pollId && v.UserId == userId, cancellationToken);
    }

    public async Task<List<Poll>> GetActivePollsAsync(CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        return await _dbContext.Polls
            .Include(p => p.Options)
            .ThenInclude(o => o.Votes)
            .Where(p => p.IsActive && (!p.ExpiresAt.HasValue || p.ExpiresAt > now))
            .OrderByDescending(p => p.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<(List<Poll> Items, long TotalCount)> GetActivePollsPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        var now = DateTime.UtcNow;
        var query = _dbContext.Polls
            .Include(p => p.Options)
            .ThenInclude(o => o.Votes)
            .Where(p => p.IsActive && (!p.ExpiresAt.HasValue || p.ExpiresAt > now))
            .OrderByDescending(p => p.CreatedAt);

        var totalCount = await query.LongCountAsync(cancellationToken);
        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }
}
