using Asp.Versioning;
using Microsoft.AspNetCore.Authorization;

namespace SocialMedia.API;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
[Authorize(Roles = "Admin")]
public class ModerationController : ControllerBase
{
    private readonly IDispatcher _dispatcher;

    public ModerationController(IDispatcher dispatcher)
    {
        _dispatcher = dispatcher;
    }

    [HttpDelete("reported-content")]
    public async Task<IActionResult> DeleteReportedContent([FromQuery] int minReports, CancellationToken cancellationToken)
    {
        var command = new DeleteReportedContentCommand(minReports);
        var deletedCount = await _dispatcher.Send<DeleteReportedContentCommand, int>(command, cancellationToken);
        return Ok(new { deletedCount = deletedCount });
    }

}
