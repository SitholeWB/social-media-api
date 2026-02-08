namespace SocialMedia.API.Controllers;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/stats")]
[ApiController]
[Authorize]
public class StatsController(IDispatcher dispatcher) : ControllerBase
{
    [HttpGet("history")]
    public async Task<IActionResult> GetDashboardStats([FromQuery] int? count, CancellationToken cancellationToken)
    {
        var input = count > 0 ? new GetStatsHistoryQuery(count.Value) : new GetStatsHistoryQuery();
        var result = await dispatcher.QueryAsync<GetStatsHistoryQuery, Result<StatsHistoryDto>>(input, cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpGet("weekly")]
    public async Task<IActionResult> GetWeeklyStats([FromQuery] DateTime? date, CancellationToken cancellationToken)
    {
        var targetDate = date ?? DateTime.UtcNow;
        var result = await dispatcher.QueryAsync<RetrieveStatsQuery, StatsRecord?>(new RetrieveStatsQuery(StatsType.Weekly, targetDate), cancellationToken);
        return Ok(result);
    }

    [HttpGet("monthly")]
    public async Task<IActionResult> GetMonthlyStats([FromQuery] DateTime? date, CancellationToken cancellationToken)
    {
        var targetDate = date ?? DateTime.UtcNow;
        var result = await dispatcher.QueryAsync<RetrieveStatsQuery, StatsRecord?>(new RetrieveStatsQuery(StatsType.Monthly, targetDate), cancellationToken);
        return Ok(result);
    }
}