namespace SocialMedia.Application;

public class RetrieveStatsQueryHandler(IStatsRepository statsRepository) : IQueryHandler<RetrieveStatsQuery, StatsRecord?>
{
    public async Task<StatsRecord?> Handle(RetrieveStatsQuery request, CancellationToken cancellationToken)
    {
        return await statsRepository.GetCurrentStatsRecordAsync(request.Type, request.Date, cancellationToken);
    }
}