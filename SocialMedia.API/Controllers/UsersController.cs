namespace SocialMedia.API;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/users")]
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

    [HttpPut("{userId}")]
    public async Task<IActionResult> UpdateUser(Guid userId, [FromBody] UpdateUserRequest request, CancellationToken cancellationToken)
    {
        var command = new UpdateUserCommand(userId, request);
        var result = await _dispatcher.Send<UpdateUserCommand, bool>(command, cancellationToken);
        return Ok(result);
    }

    [HttpPost("{userId}/change-password")]
    public async Task<IActionResult> ChangePassword(Guid userId, [FromBody] ChangePasswordRequest request, CancellationToken cancellationToken)
    {
        var command = new ChangePasswordCommand(userId, request);
        var result = await _dispatcher.Send<ChangePasswordCommand, bool>(command, cancellationToken);
        return Ok(result);
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