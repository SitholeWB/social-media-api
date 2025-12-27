namespace SocialMedia.Application;

public interface IDashboardStatsRepository
{
    Task<DashboardStatsDto> GetDashboardStatsAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken);
}