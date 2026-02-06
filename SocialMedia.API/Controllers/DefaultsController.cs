namespace SocialMedia.API;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/defaults")]
[ApiController]
[Authorize]
public class DefaultsController : ControllerBase
{
    private readonly IDispatcher _dispatcher;

    public DefaultsController(IDispatcher dispatcher)
    {
        _dispatcher = dispatcher;
    }

    [HttpPost("init")]
    [Authorize(Roles = "Admin")]
    public async Task<IActionResult> BanUser(CancellationToken cancellationToken)
    {
        var command = new CreateDefaultGroupsCommand(DefaultConstants.DEFAULT_GROUPS);
        var result = await _dispatcher.SendAsync<CreateDefaultGroupsCommand, string>(command, cancellationToken);
        return Ok(result);
    }
}