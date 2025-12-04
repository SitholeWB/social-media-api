namespace SocialMedia.Infrastructure;

public class GroupRepository : Repository<Group>, IGroupRepository
{
    private readonly SocialMediaReadDbContext _context;

    public GroupRepository(SocialMediaDbContext dbContext, SocialMediaReadDbContext readContext) : base(dbContext)
    {
        _context = readContext;
    }

    public async Task<(List<Group> Items, long TotalCount)> GetGroupsPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = _context.Groups
            .AsNoTracking()
            .OrderByDescending(g => g.CreatedAt);

        var totalCount = await query.LongCountAsync(cancellationToken);
        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }
}