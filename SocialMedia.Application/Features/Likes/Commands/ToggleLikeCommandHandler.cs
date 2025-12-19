namespace SocialMedia.Application;

public class ToggleLikeCommandHandler : ICommandHandler<ToggleLikeCommand, bool>
{
    private readonly ILikeRepository _likeRepository;
    private readonly INotificationRepository _notificationRepository;
    private readonly IPostRepository _postRepository;
    private readonly ICommentRepository _commentRepository;
    private readonly IUserActivityRepository _userActivityRepository;
    private readonly IDispatcher _dispatcher;

    public ToggleLikeCommandHandler(
        ILikeRepository likeRepository,
        INotificationRepository notificationRepository,
        IPostRepository postRepository,
        ICommentRepository commentRepository,
        IUserActivityRepository userActivityRepository,
        IDispatcher dispatcher)
    {
        _likeRepository = likeRepository;
        _notificationRepository = notificationRepository;
        _postRepository = postRepository;
        _commentRepository = commentRepository;
        _userActivityRepository = userActivityRepository;
        _dispatcher = dispatcher;
    }

    public async Task<bool> Handle(ToggleLikeCommand command, CancellationToken cancellationToken)
    {
        Like? existingLike = null;

        if (command.PostId.HasValue)
        {
            existingLike = await _likeRepository.GetByPostIdAndUserIdAsync(command.PostId.Value, command.UserId.GetValueOrDefault(), cancellationToken);
        }
        else if (command.CommentId.HasValue)
        {
            existingLike = await _likeRepository.GetByCommentIdAndUserIdAsync(command.CommentId.Value, command.UserId.GetValueOrDefault(), cancellationToken);
        }

        // Validate entity exists before creating new like
        if (existingLike == null)
        {
            if (command.PostId.HasValue)
            {
                var post = await _postRepository.GetByIdAsync(command.PostId.Value, cancellationToken);
                if (post == null)
                {
                    return false; // Post doesn't exist
                }
            }
            else if (command.CommentId.HasValue)
            {
                var comment = await _commentRepository.GetByIdAsync(command.CommentId.Value, cancellationToken);
                if (comment == null)
                {
                    return false; // Comment doesn't exist
                }
            }
        }

        var toggleLikeType = ToggleLikeType.Added;
        var oldEmoji = existingLike?.Emoji ?? string.Empty;

        var userId = command.UserId.GetValueOrDefault();
        var userActivity = await _userActivityRepository.GetByUserIdAsync(userId, cancellationToken);
        if (userActivity == null)
        {
            userActivity = new UserActivity { UserId = userId };
            await _userActivityRepository.AddAsync(userActivity, cancellationToken);
        }

        if (existingLike != null)
        {
            if (existingLike.Emoji == command.Emoji)
            {
                // Toggle off
                await _likeRepository.DeleteAsync(existingLike, cancellationToken);
                userActivity.RemoveReaction(command.PostId ?? command.CommentId!.Value, command.PostId.HasValue ? "Post" : "Comment");
                await _userActivityRepository.UpdateAsync(userActivity, cancellationToken);
                toggleLikeType = ToggleLikeType.Removed;
            }
            else
            {
                // Update emoji
                existingLike.Emoji = command.Emoji;
                existingLike.LastModifiedAt = DateTimeOffset.Now;
                await _likeRepository.UpdateAsync(existingLike, cancellationToken);
                userActivity.AddOrUpdateReaction(command.PostId ?? command.CommentId!.Value, command.PostId.HasValue ? "Post" : "Comment", command.Emoji);
                await _userActivityRepository.UpdateAsync(userActivity, cancellationToken);
                toggleLikeType = ToggleLikeType.Updated;
            }
        }
        else
        {
            // Create new
            existingLike = new Like
            {
                UserId = command.UserId.GetValueOrDefault(),
                PostId = command.PostId,
                CommentId = command.CommentId,
                Emoji = command.Emoji,
                Username = command.Username ?? "unknown",
            };
            await _likeRepository.AddAsync(existingLike, cancellationToken);

            userActivity.AddOrUpdateReaction(command.PostId ?? command.CommentId!.Value, command.PostId.HasValue ? "Post" : "Comment", command.Emoji);
            await _userActivityRepository.UpdateAsync(userActivity, cancellationToken);

            // Create Notification
            if (command.PostId.HasValue)
            {
                var post = await _postRepository.GetByIdAsync(command.PostId.Value, cancellationToken);
                if (post != null && post.AuthorId != command.UserId)
                {
                    await _notificationRepository.AddAsync(new Notification
                    {
                        UserId = post.AuthorId,
                        Message = $"{command.Username} liked your post",
                        Type = NotificationType.LikePost,
                        RelatedId = command.PostId.Value,
                        IsRead = false,
                        CreatedAt = DateTime.UtcNow
                    }, cancellationToken);
                }
            }
            else if (command.CommentId.HasValue)
            {
                var comment = await _commentRepository.GetByIdAsync(command.CommentId.Value, cancellationToken);
                if (comment != null && comment.AuthorId != command.UserId)
                {
                    await _notificationRepository.AddAsync(new Notification
                    {
                        UserId = comment.AuthorId,
                        Message = $"{command.Username} liked your comment",
                        Type = NotificationType.LikeComment,
                        RelatedId = command.CommentId.Value,
                        IsRead = false,
                        CreatedAt = DateTime.UtcNow
                    }, cancellationToken);
                }
            }
        }
        if (existingLike.CommentId.HasValue)
        {
            await _dispatcher.Publish(new CommentLikeAddedEvent(existingLike, toggleLikeType, oldEmoji), cancellationToken);
        }
        else if (existingLike.PostId.HasValue)
        {
            await _dispatcher.Publish(new PostLikeAddedEvent(existingLike, toggleLikeType, oldEmoji), cancellationToken);
        }
        return true; // Added
    }
}