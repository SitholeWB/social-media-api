namespace SocialMedia.Application;

public record GetStatsHistoryQuery(int Count = 12) : IQuery<Result<StatsHistoryDto>>;