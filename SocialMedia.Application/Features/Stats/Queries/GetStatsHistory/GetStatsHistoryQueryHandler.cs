namespace SocialMedia.Application;

public class GetStatsHistoryQueryHandler(IStatsRepository repository)
    : IQueryHandler<GetStatsHistoryQuery, Result<StatsHistoryDto>>
{
    public async Task<Result<StatsHistoryDto>> HandleAsync(GetStatsHistoryQuery request, CancellationToken cancellationToken)
    {
        var stats = await repository.GetCurrentStatsHistoryAsync(request.Count, cancellationToken);
        return Result<StatsHistoryDto>.Success(stats);
    }
}