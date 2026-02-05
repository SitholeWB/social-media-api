namespace SocialMedia.API.Controllers;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/stats")]
[ApiController]
[Authorize]
public class StatsController(IDispatcher dispatcher) : ControllerBase
{
    [HttpGet("dashboard")]
    public async Task<IActionResult> GetDashboardStats([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate, CancellationToken cancellationToken)
    {
        var result = await dispatcher.Query<GetDashboardStatsQuery, Result<DashboardStatsDto>>(new GetDashboardStatsQuery(startDate, endDate), cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }

    [HttpGet("weekly")]
    public async Task<IActionResult> GetWeeklyStats([FromQuery] DateTime? date, CancellationToken cancellationToken)
    {
        var targetDate = date ?? DateTime.UtcNow;
        var result = await dispatcher.Query<RetrieveStatsQuery, StatsRecord?>(new RetrieveStatsQuery(SocialMedia.Domain.StatsType.Weekly, targetDate), cancellationToken);
        return Ok(result);
    }

    [HttpGet("monthly")]
    public async Task<IActionResult> GetMonthlyStats([FromQuery] DateTime? date, CancellationToken cancellationToken)
    {
        var targetDate = date ?? DateTime.UtcNow;
        var result = await dispatcher.Query<RetrieveStatsQuery, StatsRecord?>(new RetrieveStatsQuery(SocialMedia.Domain.StatsType.Monthly, targetDate), cancellationToken);
        return Ok(result);
    }
}