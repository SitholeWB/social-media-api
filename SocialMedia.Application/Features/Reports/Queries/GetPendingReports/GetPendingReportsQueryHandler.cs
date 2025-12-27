namespace SocialMedia.Application;

public class GetPendingReportsQueryHandler : IQueryHandler<GetPendingReportsQuery, PagedResult<ReportDto>>
{
    private readonly IReportRepository _reportRepository;

    public GetPendingReportsQueryHandler(IReportRepository reportRepository)
    {
        _reportRepository = reportRepository;
    }

    public async Task<PagedResult<ReportDto>> Handle(GetPendingReportsQuery query, CancellationToken cancellationToken)
    {
        var (reports, totalCount) = await _reportRepository.GetPendingReportsPagedAsync(query.PageNumber, query.PageSize, cancellationToken);
        var dtos = reports.Select(r => new ReportDto
        {
            Id = r.Id,
            ReporterId = r.ReporterId,
            Reason = r.Reason,
            Status = r.Status,
            PostId = r.PostId,
            CommentId = r.CommentId,
            CreatedAt = r.CreatedAt
        }).ToList();

        return new PagedResult<ReportDto>(dtos, totalCount, query.PageNumber, query.PageSize);
    }
}