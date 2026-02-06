namespace SocialMedia.API;

[ApiVersion("1.0")]
[Route("api/v{version:apiVersion}/notifications")]
[ApiController]
[Authorize]
public class NotificationsController : ControllerBase
{
    private readonly IDispatcher _dispatcher;

    public NotificationsController(IDispatcher dispatcher)
    {
        _dispatcher = dispatcher;
    }

    [HttpGet("{userId}")]
    public async Task<IActionResult> GetNotifications(Guid userId, CancellationToken cancellationToken)
    {
        // In a real app, we should check if the current user is allowed to view these notifications
        var query = new GetNotificationsQuery(userId);
        var result = await _dispatcher.QueryAsync<GetNotificationsQuery, List<NotificationDto>>(query, cancellationToken);
        return Ok(result);
    }
}