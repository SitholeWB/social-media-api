namespace SocialMedia.API.Controllers;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/feedback")]
[ApiController]
[Authorize]
public class FeedbackController : ControllerBase
{
    private readonly IDispatcher _dispatcher;

    public FeedbackController(IDispatcher dispatcher)
    {
        _dispatcher = dispatcher;
    }

    [HttpPost]
    public async Task<IActionResult> SubmitFeedback([FromBody] SubmitFeedbackRequest request, CancellationToken cancellationToken)
    {
        var command = new SubmitFeedbackCommand(request);
        var result = await _dispatcher.SendAsync<SubmitFeedbackCommand, bool>(command, cancellationToken);
        return Ok(result);
    }
}
