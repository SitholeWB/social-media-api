namespace SocialMedia.Application;

public class RetrieveStatsQueryHandler(IStatsRepository statsRepository) : IQueryHandler<RetrieveStatsQuery, StatsRecord?>
{
    public async Task<StatsRecord?> Handle(RetrieveStatsQuery request, CancellationToken cancellationToken)
    {
        if (request.Type == StatsType.Weekly)
        {
            return await statsRepository.GetCurrentStatsRecordAsync(request.Type, request.Date.ToWeekStartDate(), cancellationToken);
        }
        return await statsRepository.GetCurrentStatsRecordAsync(request.Type, request.Date.ToMonthStartDate(), cancellationToken);
    }
}