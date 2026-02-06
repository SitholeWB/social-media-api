namespace SocialMedia.API;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/posts")]
[ApiController]
[Authorize]
public class PostsController : ControllerBase
{
    private readonly IDispatcher _dispatcher;

    public PostsController(IDispatcher dispatcher)
    {
        _dispatcher = dispatcher;
    }

    [AllowAnonymous]
    [HttpGet("{id}")]
    public async Task<IActionResult> GetById(Guid id, CancellationToken cancellationToken)
    {
        var query = new GetPostByIdQuery(id) { UserId = this.GetUserId() };
        var post = await _dispatcher.QueryAsync<GetPostByIdQuery, PostDto?>(query, cancellationToken);
        if (post == null)
        {
            return NotFound();
        }
        return Ok(post);
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeleteCommentById(Guid id, CancellationToken cancellationToken)
    {
        var command = new DeletePostCommand(id, this.GetUserId());
        var comment = await _dispatcher.SendAsync<DeletePostCommand, bool>(command, cancellationToken);
        if (!comment)
        {
            return NotFound();
        }
        return Ok(comment);
    }

    [HttpPost("{id}/report")]
    public async Task<IActionResult> ReportPost(Guid id, [FromBody] ReportPostCommand command, CancellationToken cancellationToken)
    {
        if (id != command.PostId)
        {
            return BadRequest("Post ID mismatch");
        }

        var reportId = await _dispatcher.SendAsync<ReportPostCommand, Guid>(command, cancellationToken);
        return Ok(reportId);
    }
}