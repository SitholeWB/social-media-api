namespace SocialMedia.API;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/polls")]
[ApiController]
[Authorize]
public class PollsController : ControllerBase
{
    private readonly IDispatcher _dispatcher;

    public PollsController(IDispatcher dispatcher)
    {
        _dispatcher = dispatcher;
    }

    [HttpPost]
    public async Task<IActionResult> Create([FromBody] CreatePollCommand command, CancellationToken cancellationToken)
    {
        var pollId = await _dispatcher.Send<CreatePollCommand, Guid>(command, cancellationToken);
        return CreatedAtAction(nameof(Get), new { id = pollId }, pollId);
    }

    [HttpPost("{id}/vote")]
    public async Task<IActionResult> Vote(Guid id, [FromBody] VoteOnPollCommand command, CancellationToken cancellationToken)
    {
        if (id != command.PollId)
        {
            return BadRequest("Poll ID mismatch");
        }

        try
        {
            var success = await _dispatcher.Send<VoteOnPollCommand, bool>(command, cancellationToken);
            if (!success)
            {
                return BadRequest("Unable to vote on this poll (it may be inactive or expired).");
            }
            return Ok();
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(ex.Message);
        }
    }

    [AllowAnonymous]
    [HttpGet("active")]
    public async Task<IActionResult> GetActivePolls([FromQuery] int pageNumber = 1, [FromQuery] int pageSize = 10, CancellationToken cancellationToken = default)
    {
        var query = new GetActivePollsQuery(pageNumber, pageSize);
        var result = await _dispatcher.Query<GetActivePollsQuery, PagedResult<PollDto>>(query, cancellationToken);
        return Ok(result);
    }

    [AllowAnonymous]
    [HttpGet("{id}")]
    public async Task<IActionResult> Get(Guid id, CancellationToken cancellationToken)
    {
        var query = new GetPollQuery(Guid.Empty) { PollId = id };
        var poll = await _dispatcher.Query<GetPollQuery, PollDto?>(query, cancellationToken);
        if (poll == null)
        {
            return NotFound();
        }
        return Ok(poll);
    }

    [HttpGet("chain/verify")]
    public async Task<IActionResult> VerifyChain(CancellationToken cancellationToken)
    {
        var query = new VerifyChainQuery();
        var isValid = await _dispatcher.Query<VerifyChainQuery, bool>(query, cancellationToken);
        return Ok(new { IsValid = isValid });
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdatePoll(Guid id, [FromBody] UpdatePollCommand command, CancellationToken cancellationToken)
    {
        if (id != command.PollId)
        {
            return BadRequest();
        }

        var result = await _dispatcher.Send<UpdatePollCommand, bool>(command, cancellationToken);
        if (!result)
        {
            return NotFound();
        }
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> DeletePoll(Guid id, CancellationToken cancellationToken)
    {
        var command = new DeletePollCommand(id);
        var result = await _dispatcher.Send<DeletePollCommand, bool>(command, cancellationToken);
        if (!result)
        {
            return NotFound();
        }
        return NoContent();
    }
}