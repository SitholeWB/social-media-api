namespace SocialMedia.Application;

public record GetDashboardStatsQuery(DateTime? StartDate, DateTime? EndDate) : IQuery<Result<DashboardStatsDto>>;
