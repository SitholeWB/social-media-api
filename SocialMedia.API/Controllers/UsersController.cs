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
        await _dispatcher.SendAsync<BlockUserCommand, bool>(command, cancellationToken);
        return Ok();
    }

    [HttpPost("{userId}/ban")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> BanUser(Guid userId, [FromBody] bool isBanned, CancellationToken cancellationToken)
    {
        var command = new AdminBlockUserCommand(userId, isBanned);
        await _dispatcher.SendAsync<AdminBlockUserCommand, bool>(command, cancellationToken);
        return Ok();
    }

    [HttpPut("{userId}")]
    public async Task<IActionResult> UpdateUser(Guid userId, [FromBody] UpdateUserRequest request, CancellationToken cancellationToken)
    {
        var tokenUserId = this.GetUserId();
        if (tokenUserId != userId)
        {
            return BadRequest($"User with Id {tokenUserId} is not allowed to update details for user Id {userId}");
        }
        var command = new UpdateUserCommand(userId, request);
        var result = await _dispatcher.SendAsync<UpdateUserCommand, bool>(command, cancellationToken);
        return Ok(result);
    }

    [HttpPost("{userId}/change-password")]
    public async Task<IActionResult> ChangePassword(Guid userId, [FromBody] ChangePasswordRequest request, CancellationToken cancellationToken)
    {
        var command = new ChangePasswordCommand(userId, request);
        var result = await _dispatcher.SendAsync<ChangePasswordCommand, bool>(command, cancellationToken);
        return Ok(result);
    }

    [HttpGet("reported")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> GetReportedUsers([FromQuery] int minReports = 5, CancellationToken cancellationToken = default)
    {
        var query = new GetReportedUsersQuery(minReports);
        var result = await _dispatcher.QueryAsync<GetReportedUsersQuery, List<ReportedUserDto>>(query, cancellationToken);
        return Ok(result);
    }
}