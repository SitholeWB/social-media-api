namespace SocialMedia.Infrastructure;

public class StatsRepository(SocialMediaReadDbContext context) : IStatsRepository
{
    public async Task<StatsRecord?> GetCurrentStatsRecordAsync(StatsType type, DateTimeOffset date, CancellationToken cancellationToken)
    {
        return await context.StatsRecords
            .FirstOrDefaultAsync(s => s.StatsType == type && s.Date == date, cancellationToken);
    }

    public async Task<StatsHistoryDto> GetCurrentStatsHistoryAsync(int count, CancellationToken cancellationToken)
    {
        var weeks = await context.StatsRecords.Where(x => x.StatsType == StatsType.Weekly).OrderByDescending(s => s.Date).Take(count).ToListAsync(cancellationToken);
        var months = await context.StatsRecords.Where(x => x.StatsType == StatsType.Monthly).OrderByDescending(s => s.Date).Take(count).ToListAsync(cancellationToken);
        return new StatsHistoryDto
        {
            Months = months,
            Weeks = weeks
        };
    }

    public async Task AddAsync(StatsRecord statsRecord, CancellationToken cancellationToken)
    {
        await context.StatsRecords.AddAsync(statsRecord, cancellationToken);
        await context.SaveChangesAsync(cancellationToken);
    }

    public async Task UpdateAsync(StatsRecord statsRecord, CancellationToken cancellationToken)
    {
        context.StatsRecords.Update(statsRecord);
        await context.SaveChangesAsync(cancellationToken);
    }
}