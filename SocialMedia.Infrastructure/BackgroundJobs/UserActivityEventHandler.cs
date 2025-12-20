namespace SocialMedia.Infrastructure;

public class UserActivityEventHandler : 
    IDomainEventHandler<PostLikeAddedEvent>,
    IDomainEventHandler<CommentLikeAddedEvent>,
    IDomainEventHandler<PollVotedEvent>
{
    private readonly IServiceScopeFactory _scopeFactory;
    private readonly IDistributedCache _cache;
    private readonly ILogger<UserActivityEventHandler> _logger;

    public UserActivityEventHandler(
        IServiceScopeFactory scopeFactory,
        IDistributedCache cache,
        ILogger<UserActivityEventHandler> logger)
    {
        _scopeFactory = scopeFactory;
        _cache = cache;
        _logger = logger;
    }

    public async Task Handle(PostLikeAddedEvent notification, CancellationToken cancellationToken)
    {
        await UpdateUserReactionAsync(
            notification.Like.UserId, 
            notification.Like.PostId!.Value, 
            "Post", 
            notification.ToggleLikeType, 
            notification.Like.Emoji, 
            cancellationToken);
    }

    public async Task Handle(CommentLikeAddedEvent notification, CancellationToken cancellationToken)
    {
        await UpdateUserReactionAsync(
            notification.Like.UserId, 
            notification.Like.CommentId!.Value, 
            "Comment", 
            notification.ToggleLikeType, 
            notification.Like.Emoji, 
            cancellationToken);
    }

    public async Task Handle(PollVotedEvent notification, CancellationToken cancellationToken)
    {
        using var scope = _scopeFactory.CreateScope();
        var userActivityRepository = scope.ServiceProvider.GetRequiredService<IUserActivityRepository>();

        var userActivity = await userActivityRepository.GetByUserIdAsync(notification.UserId, cancellationToken) 
                           ?? new UserActivity { UserId = notification.UserId };
        
        if (userActivity.Id == Guid.Empty)
        {
             await userActivityRepository.AddAsync(userActivity, cancellationToken);
        }

        userActivity.AddVote(notification.PollId, notification.OptionId);
        await userActivityRepository.UpdateAsync(userActivity, cancellationToken);
        
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
        using var scope = _scopeFactory.CreateScope();
        var userActivityRepository = scope.ServiceProvider.GetRequiredService<IUserActivityRepository>();

        var userActivity = await userActivityRepository.GetByUserIdAsync(userId, cancellationToken) 
                           ?? new UserActivity { UserId = userId };

        if (userActivity.Id == Guid.Empty)
        {
             await userActivityRepository.AddAsync(userActivity, cancellationToken);
        }

        if (toggleType == ToggleLikeType.Removed)
        {
            userActivity.RemoveReaction(entityId, entityType);
        }
        else
        {
            userActivity.AddOrUpdateReaction(entityId, entityType, emoji);
        }

        await userActivityRepository.UpdateAsync(userActivity, cancellationToken);
        await InvalidateCacheAsync(userId, cancellationToken);
    }

    private async Task InvalidateCacheAsync(Guid userId, CancellationToken cancellationToken)
    {
        var cacheKey = $"user_activity_{userId}";
        await _cache.RemoveAsync(cacheKey, cancellationToken);
        _logger.LogInformation("Invalidated UserActivity cache for {UserId}", userId);
    }
}
