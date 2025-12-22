namespace SocialMedia.Infrastructure;

public class GroupRepository : Repository<Group>, IGroupRepository
{
    public GroupRepository(SocialMediaDbContext dbContext) : base(dbContext)
    {
    }

    public async Task<(List<Group> Items, long TotalCount)> GetGroupsPagedAsync(int pageNumber, int pageSize, bool includeDefaults = false, CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Groups
            .Where(g => !DefaultConstants.DEFAULT_GROUPS.Select(s => s.Id).Contains(g.Id))
            .AsNoTracking()
            .OrderByDescending(g => g.CreatedAt);
        if (includeDefaults)
        {
            query = _dbContext.Groups
            .AsNoTracking()
            .OrderByDescending(g => g.CreatedAt);
        }

        var totalCount = await query.LongCountAsync(cancellationToken);
        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }
}