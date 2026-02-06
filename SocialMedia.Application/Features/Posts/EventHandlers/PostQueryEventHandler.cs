namespace SocialMedia.Application;

public class PostQueryEventHandler :
    IEventHandler<PostQueryEvent>
{
    private readonly IUserActivityRepository _userActivityRepository;

    public PostQueryEventHandler(
        IUserActivityRepository userActivityRepository)
    {
        _userActivityRepository = userActivityRepository;
    }

    public async Task HandleAsync(PostQueryEvent notification, CancellationToken cancellationToken)
    {
        await _userActivityRepository.UpdateUserLastSeenAsync(notification.UserId, cancellationToken);
    }
}