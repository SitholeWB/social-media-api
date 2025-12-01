

namespace SocialMedia.API;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IDispatcher _dispatcher;

    public UsersController(IDispatcher dispatcher)
    {
        _dispatcher = dispatcher;
    }

    [HttpPost("block")]
    public async Task<IActionResult> BlockUser([FromBody] BlockUserCommand command, CancellationToken cancellationToken)
    {
        await _dispatcher.Send<BlockUserCommand, bool>(command, cancellationToken);
        return Ok();
    }

    [HttpPost("{userId}/ban")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> BanUser(Guid userId, [FromBody] bool isBanned, CancellationToken cancellationToken)
    {
        var command = new AdminBlockUserCommand(userId, isBanned);
        await _dispatcher.Send<AdminBlockUserCommand, bool>(command, cancellationToken);
        return Ok();
    }

    [HttpGet("reported")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetReportedUsers([FromQuery] int minReports = 5, CancellationToken cancellationToken = default)
    {
        var query = new GetReportedUsersQuery(minReports);
        var result = await _dispatcher.Query<GetReportedUsersQuery, List<ReportedUserDto>>(query, cancellationToken);
        return Ok(result);
    }

}
