namespace SocialMedia.Application;

public class GetDashboardStatsQueryHandler(IDashboardStatsRepository repository)
    : IQueryHandler<GetDashboardStatsQuery, Result<DashboardStatsDto>>
{
    public async Task<Result<DashboardStatsDto>> Handle(GetDashboardStatsQuery request, CancellationToken cancellationToken)
    {
        var startDate = request.StartDate ?? DateTime.UtcNow.AddDays(-30);
        var endDate = request.EndDate ?? DateTime.UtcNow;

        var stats = await repository.GetDashboardStatsAsync(startDate, endDate, cancellationToken);

        return Result<DashboardStatsDto>.Success(stats);
    }
}