using SocialMedia.Application.Stats.DTOs;

namespace SocialMedia.Application.Common.Interfaces;

public interface IDashboardStatsRepository
{
    Task<DashboardStatsDto> GetDashboardStatsAsync(DateTime startDate, DateTime endDate, CancellationToken cancellationToken);
}
