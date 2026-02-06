using Microsoft.Extensions.Logging;

namespace SocialMedia.Application;

public class UserActivityEventHandler :
    IEventHandler<PostLikeAddedEvent>,
    IEventHandler<CommentLikeAddedEvent>,
    IEventHandler<PollVotedEvent>
{
    private readonly IUserActivityRepository _userActivityRepository;
    private readonly ILogger<UserActivityEventHandler> _logger;

    public UserActivityEventHandler(
        IUserActivityRepository userActivityRepository,
        ILogger<UserActivityEventHandler> logger)
    {
        _userActivityRepository = userActivityRepository;
        _logger = logger;
    }

    public async Task HandleAsync(PostLikeAddedEvent notification, CancellationToken cancellationToken)
    {
        await UpdateUserReactionAsync(
            notification.Like.UserId,
            notification.Like.PostId!.Value,
            "Post",
            notification.ToggleLikeType,
            notification.Like.Emoji,
            cancellationToken);
    }

    public async Task HandleAsync(CommentLikeAddedEvent notification, CancellationToken cancellationToken)
    {
        await UpdateUserReactionAsync(
            notification.Like.UserId,
            notification.Like.CommentId!.Value,
            "Comment",
            notification.ToggleLikeType,
            notification.Like.Emoji,
            cancellationToken);
    }

    public async Task HandleAsync(PollVotedEvent notification, CancellationToken cancellationToken)
    {
        var userActivity = await _userActivityRepository.GetByUserIdAsync(notification.UserId, true, cancellationToken)
                           ?? new UserActivity { UserId = notification.UserId };

        if (userActivity.Id == Guid.Empty)
        {
            await _userActivityRepository.AddAsync(userActivity, cancellationToken);
        }

        userActivity.AddVote(notification.PollId, notification.OptionId);
        await _userActivityRepository.UpdateAsync(userActivity, cancellationToken);

        await InvalidateCacheAsync(notification.UserId, cancellationToken);
    }

    private async Task UpdateUserReactionAsync(
        Guid userId,
        Guid entityId,
        string entityType,
        ToggleLikeType toggleType,
        string emoji,
        CancellationToken cancellationToken)
    {
        var userActivity = await _userActivityRepository.GetByUserIdAsync(userId, true, cancellationToken);

        if (userActivity == default)
        {
            userActivity = new UserActivity { UserId = userId };
            await _userActivityRepository.AddAsync(userActivity, cancellationToken);
        }

        if (toggleType == ToggleLikeType.Removed)
        {
            userActivity.RemoveReaction(entityId, entityType);
        }
        else
        {
            userActivity.AddOrUpdateReaction(entityId, entityType, emoji);
        }

        await _userActivityRepository.UpdateAsync(userActivity, cancellationToken);
        await InvalidateCacheAsync(userId, cancellationToken);
    }

    private async Task InvalidateCacheAsync(Guid userId, CancellationToken cancellationToken)
    {
        await _userActivityRepository.RefreshCacheAsync(userId, cancellationToken);
        _logger.LogInformation("Invalidated UserActivity cache for {UserId}", userId);
    }
}