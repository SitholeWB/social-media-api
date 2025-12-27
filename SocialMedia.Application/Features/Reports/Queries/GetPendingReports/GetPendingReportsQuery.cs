namespace SocialMedia.Application;

public record GetPendingReportsQuery() : IQuery<PagedResult<ReportDto>>
{
    public int PageNumber { get; set; } = 1;
    public int PageSize { get; set; } = 10;
}