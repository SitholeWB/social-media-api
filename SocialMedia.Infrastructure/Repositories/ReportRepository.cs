
namespace SocialMedia.Infrastructure;

public class ReportRepository : IReportRepository
{
    private readonly SocialMediaDbContext _dbContext;

    public ReportRepository(SocialMediaDbContext dbContext)
    {
        _dbContext = dbContext;
    }

    public async Task AddAsync(Report report, CancellationToken cancellationToken = default)
    {
        await _dbContext.Reports.AddAsync(report, cancellationToken);
        await _dbContext.SaveChangesAsync(cancellationToken);
    }

    public async Task<List<Report>> GetReportsByPostIdAsync(Guid postId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Reports
            .Where(r => r.PostId == postId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Report>> GetReportsByCommentIdAsync(Guid commentId, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Reports
            .Where(r => r.CommentId == commentId)
            .OrderByDescending(r => r.CreatedAt)
            .ToListAsync(cancellationToken);
    }

    public async Task<(List<Report> Items, long TotalCount)> GetPendingReportsPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default)
    {
        var query = _dbContext.Reports
            .Include(r => r.Post)
            .Include(r => r.Comment)
            .Where(r => r.Status == ReportStatus.Pending)
            .OrderByDescending(r => r.CreatedAt);

        var totalCount = await query.LongCountAsync(cancellationToken);
        var items = await query
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (items, totalCount);
    }

    public async Task<List<Guid>> GetPostIdsWithReportsAboveThresholdAsync(int minReports, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Reports
            .Where(r => r.PostId != null)
            .GroupBy(r => r.PostId)
            .Where(g => g.Count() > minReports)
            .Select(g => g.Key!.Value)
            .ToListAsync(cancellationToken);
    }

    public async Task<List<Guid>> GetCommentIdsWithReportsAboveThresholdAsync(int minReports, CancellationToken cancellationToken = default)
    {
        return await _dbContext.Reports
            .Where(r => r.CommentId != null)
            .GroupBy(r => r.CommentId)
            .Where(g => g.Count() > minReports)
            .Select(g => g.Key!.Value)
            .ToListAsync(cancellationToken);
    }

    public async Task<Dictionary<Guid, int>> GetPostReportCountsByAuthorAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Reports
            .Where(r => r.PostId != null)
            .Include(r => r.Post)
            .GroupBy(r => r.Post!.AuthorId)
            .Select(g => new { AuthorId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.AuthorId, x => x.Count, cancellationToken);
    }

    public async Task<Dictionary<Guid, int>> GetCommentReportCountsByAuthorAsync(CancellationToken cancellationToken = default)
    {
        return await _dbContext.Reports
            .Where(r => r.CommentId != null)
            .Include(r => r.Comment)
            .GroupBy(r => r.Comment!.AuthorId)
            .Select(g => new { AuthorId = g.Key, Count = g.Count() })
            .ToDictionaryAsync(x => x.AuthorId, x => x.Count, cancellationToken);
    }
}
