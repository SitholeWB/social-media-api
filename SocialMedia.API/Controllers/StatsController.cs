using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialMedia.Application.Stats.Queries.GetDashboardStats;
using SocialMedia.Application.Stats.DTOs;
using SocialMedia.Application.Common.Models;

namespace SocialMedia.API.Controllers;

[Authorize]
[ApiController]
[Route("api/[controller]")]
public class StatsController(Dispatcher dispatcher) : ControllerBase
{
    [HttpGet("dashboard")]
    public async Task<IActionResult> GetDashboardStats([FromQuery] DateTime? startDate, [FromQuery] DateTime? endDate, CancellationToken cancellationToken)
    {
        var result = await dispatcher.Query<GetDashboardStatsQuery, Result<DashboardStatsDto>>(new GetDashboardStatsQuery(startDate, endDate), cancellationToken);
        return result.IsSuccess ? Ok(result.Value) : BadRequest(result.Error);
    }
}
