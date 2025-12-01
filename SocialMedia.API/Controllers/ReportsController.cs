


namespace SocialMedia.API;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
public class ReportsController : ControllerBase
{
    private readonly IDispatcher _dispatcher;

    public ReportsController(IDispatcher dispatcher)
    {
        _dispatcher = dispatcher;
    }

    [HttpGet("pending")]
    public async Task<IActionResult> GetPendingReports([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, CancellationToken cancellationToken = default)
    {
        var query = new GetPendingReportsQuery { PageNumber = pageNumber, PageSize = pageSize };
        var result = await _dispatcher.Query<GetPendingReportsQuery, PagedResult<ReportDto>>(query, cancellationToken);
        return Ok(result);
    }
}
