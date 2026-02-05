namespace SocialMedia.Application;

public interface IStatsRepository
{
    Task<StatsRecord?> GetCurrentStatsRecordAsync(StatsType type, DateTimeOffset date, CancellationToken cancellationToken);

    Task AddAsync(StatsRecord statsRecord, CancellationToken cancellationToken);

    Task UpdateAsync(StatsRecord statsRecord, CancellationToken cancellationToken);
}