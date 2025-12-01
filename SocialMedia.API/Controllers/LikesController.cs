

namespace SocialMedia.API;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/[controller]")]
[ApiController]
public class LikesController : ControllerBase
{
    private readonly IDispatcher _dispatcher;

    public LikesController(IDispatcher dispatcher)
    {
        _dispatcher = dispatcher;
    }

    [HttpPost("toggle")]
    public async Task<IActionResult> ToggleLike([FromBody] ToggleLikeCommand command, CancellationToken cancellationToken)
    {
        var result = await _dispatcher.Send<ToggleLikeCommand, bool>(command, cancellationToken);
        return Ok(result);
    }
}
