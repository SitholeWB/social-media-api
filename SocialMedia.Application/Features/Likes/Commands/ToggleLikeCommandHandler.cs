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
            existingLike = await _likeRepository.GetByPostIdAndUserIdAsync(command.PostId.Value, command.UserId, cancellationToken);
        }
        else if (command.CommentId.HasValue)
        {
            existingLike = await _likeRepository.GetByCommentIdAndUserIdAsync(command.CommentId.Value, command.UserId, cancellationToken);
        }

        if (existingLike != null)
        {
            if (existingLike.Emoji == command.Emoji)
            {
                // Toggle off
                await _likeRepository.DeleteAsync(existingLike, cancellationToken);
                return false; // Removed
            }
            else
            {
                // Update emoji
                existingLike.Emoji = command.Emoji;
                await _likeRepository.UpdateAsync(existingLike, cancellationToken);
                return true; // Updated
            }
        }
        else
        {
            // Create new
            var newLike = new Like
            {
                UserId = command.UserId,
                PostId = command.PostId,
                CommentId = command.CommentId,
                Emoji = command.Emoji,
                Username = command.username
            };
            await _likeRepository.AddAsync(newLike, cancellationToken);

            // Create Notification
            if (command.PostId.HasValue)
            {
                var post = await _postRepository.GetByIdAsync(command.PostId.Value, cancellationToken);
                if (post != null && post.AuthorId != command.UserId)
                {
                    await _notificationRepository.AddAsync(new Notification
                    {
                        UserId = post.AuthorId,
                        Message = $"{command.username} liked your post",
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
                        Message = $"{command.username} liked your comment",
                        Type = NotificationType.LikeComment,
                        RelatedId = command.CommentId.Value,
                        IsRead = false,
                        CreatedAt = DateTime.UtcNow
                    }, cancellationToken);
                }
            }

            // Publish Event
            await _dispatcher.Publish(new LikeAddedEvent(newLike), cancellationToken);

            return true; // Added
        }
    }
}