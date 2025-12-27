namespace SocialMedia.Domain;

public interface IReportRepository
{
    Task AddAsync(Report report, CancellationToken cancellationToken = default);

    Task<List<Report>> GetReportsByPostIdAsync(Guid postId, CancellationToken cancellationToken = default);

    Task<List<Report>> GetReportsByCommentIdAsync(Guid commentId, CancellationToken cancellationToken = default);

    Task<(List<Report> Items, long TotalCount)> GetPendingReportsPagedAsync(int pageNumber, int pageSize, CancellationToken cancellationToken = default);

    Task<List<Guid>> GetPostIdsWithReportsAboveThresholdAsync(int minReports, CancellationToken cancellationToken = default);

    Task<List<Guid>> GetCommentIdsWithReportsAboveThresholdAsync(int minReports, CancellationToken cancellationToken = default);

    Task<Dictionary<Guid, int>> GetPostReportCountsByAuthorAsync(CancellationToken cancellationToken = default);

    Task<Dictionary<Guid, int>> GetCommentReportCountsByAuthorAsync(CancellationToken cancellationToken = default);
}