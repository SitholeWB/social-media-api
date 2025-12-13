namespace SocialMedia.Application;

public class ToggleLikeCommandHandler : ICommandHandler<ToggleLikeCommand, bool>
{
    private readonly ILikeRepository _likeRepository;
    private readonly INotificationRepository _notificationRepository;
    private readonly IPostRepository _postRepository;
    private readonly ICommentRepository _commentRepository;
    private readonly IDispatcher _dispatcher;

    public ToggleLikeCommandHandler(
        ILikeRepository likeRepository,
        INotificationRepository notificationRepository,
        IPostRepository postRepository,
        ICommentRepository commentRepository,
        IDispatcher dispatcher)
    {
        _likeRepository = likeRepository;
        _notificationRepository = notificationRepository;
        _postRepository = postRepository;
        _commentRepository = commentRepository;
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
        var toggleLikeType = ToggleLikeType.Added;
        if (existingLike != null)
        {
            if (existingLike.Emoji == command.Emoji)
            {
                // Toggle off
                await _likeRepository.DeleteAsync(existingLike, cancellationToken);
                toggleLikeType = ToggleLikeType.Removed;
            }
            else
            {
                // Update emoji
                existingLike.Emoji = command.Emoji;
                await _likeRepository.UpdateAsync(existingLike, cancellationToken);
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
            await _dispatcher.Publish(new CommentLikeAddedEvent(existingLike, toggleLikeType), cancellationToken);
        }
        else if (existingLike.PostId.HasValue)
        {
            await _dispatcher.Publish(new PostLikeAddedEvent(existingLike, toggleLikeType), cancellationToken);
        }
        return toggleLikeType == ToggleLikeType.Added; // Added
    }
}