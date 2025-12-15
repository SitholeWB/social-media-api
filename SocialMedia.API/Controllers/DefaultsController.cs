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
        var defaultGroups = new List<DefaultGroupDto>
        {
            new(Guid.Parse("00000000-0000-0000-0000-000000000001"), "Home", true, true, "This is the home screen or page"),
            new(Guid.Parse("00000000-0000-0000-0000-000000000002"), "Fans", true, true, "This is the Fans group or page"),
            new(Guid.Parse("00000000-0000-0000-0000-000000000003"), "Offers", true, true, "This is the group or screen or page for our sponsors latest deals"),
            new(Guid.Parse("00000000-0000-0000-0000-000000000004"), "Latest News", true, true, "Latest new group"),
        };
        var command = new CreateDefaultGroupsCommand(defaultGroups);
        var result = await _dispatcher.Send<CreateDefaultGroupsCommand, string>(command, cancellationToken);
        return Ok(result);
    }
}