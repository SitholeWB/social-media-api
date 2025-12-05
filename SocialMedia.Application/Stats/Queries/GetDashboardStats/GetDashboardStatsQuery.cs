using SocialMedia.Application.Common.Models;
using SocialMedia.Application.Stats.DTOs;

namespace SocialMedia.Application.Stats.Queries.GetDashboardStats;

public record GetDashboardStatsQuery(DateTime? StartDate, DateTime? EndDate) : IQuery<Result<DashboardStatsDto>>;
