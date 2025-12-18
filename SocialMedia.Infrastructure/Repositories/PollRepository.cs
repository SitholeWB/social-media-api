
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
        return await _dbContext.Set<PollVoteRecord>()
            .AnyAsync(r => r.PollId == pollId && r.UserId == userId, cancellationToken);
    }

    public async Task AddVoteRecordAsync(PollVoteRecord record, CancellationToken cancellationToken = default)
    {
        await _dbContext.Set<PollVoteRecord>().AddAsync(record, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<List<Poll>> GetActivePollsAsync(Guid groupId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Polls
            .Include(p => p.Options)
            .Where(p => p.GroupId == groupId && p.IsActive && (p.ExpiresAt == null || p.ExpiresAt > DateTime.UtcNow))
            .ToListAsync(cancellationToken);
    }

    public async Task<(List<Poll> Items, long TotalCount)> GetActivePollsPagedAsync(Guid groupId, int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Polls
            .Include(p => p.Options)
            .Where(p => p.GroupId == groupId && p.IsActive && (p.ExpiresAt == null || p.ExpiresAt > DateTime.UtcNow));

        var totalCount = await query.LongCountAsync(cancellationToken);
        var items = await query
            .OrderByDescending(p => p.CreatedAt)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task DeleteAsync(Poll poll, CancellationToken cancellationToken = default)
    {
        _dbContext.Polls.Remove(poll);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }
}
