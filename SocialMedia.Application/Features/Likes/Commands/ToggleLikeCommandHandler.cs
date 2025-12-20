namespace SocialMedia.Application;

public class ToggleLikeCommandHandler : ICommandHandler<ToggleLikeCommand, bool>
{
    private readonly ILikeRepository _likeRepository;
    private readonly INotificationRepository _notificationRepository;
    private readonly IPostRepository _postRepository;
    private readonly ICommentRepository _commentRepository;
    private readonly IDispatcher _dispatcher;
    private readonly IUserRepository _userRepository;

    public ToggleLikeCommandHandler(
        ILikeRepository likeRepository,
        INotificationRepository notificationRepository,
        IPostRepository postRepository,
        ICommentRepository commentRepository,
        IDispatcher dispatcher,
        IUserRepository userRepository)
    {
        _likeRepository = likeRepository;
        _notificationRepository = notificationRepository;
        _postRepository = postRepository;
        _commentRepository = commentRepository;
        _dispatcher = dispatcher;
        _userRepository = userRepository;
    }

    public async Task<bool> Handle(ToggleLikeCommand request, CancellationToken cancellationToken)
    {
        Like? existingLike = null;
        if (request.PostId.HasValue)
        {
            existingLike = await _likeRepository.GetByPostIdAndUserIdAsync(request.PostId.Value, request.UserId.GetValueOrDefault(), cancellationToken);
        }
        else if (request.CommentId.HasValue)
        {
            existingLike = await _likeRepository.GetByCommentIdAndUserIdAsync(request.CommentId.Value, request.UserId.GetValueOrDefault(), cancellationToken);
        }

        var type = existingLike == null ? ToggleLikeType.Added : ToggleLikeType.Removed;
        string oldEmoji = string.Empty;

        if (existingLike == null)
        {
            var like = new Like
            {
                UserId = request.UserId.GetValueOrDefault(),
                PostId = request.PostId,
                CommentId = request.CommentId,
                Emoji = request.Emoji,
                Username = request.Username ?? "Unknown",
                CreatedAt = DateTime.UtcNow
            };
            await _likeRepository.AddAsync(like, cancellationToken);
            existingLike = like;

             // Create Notification
            if (request.PostId.HasValue)
            {
                var post = await _postRepository.GetByIdAsync(request.PostId.Value, cancellationToken);
                if (post != null && post.AuthorId != request.UserId)
                {
                    await _notificationRepository.AddAsync(new Notification
                    {
                        UserId = post.AuthorId,
                        Message = $"{request.Username} liked your post",
                        Type = NotificationType.LikePost,
                        RelatedId = request.PostId.Value,
                        IsRead = false,
                        CreatedAt = DateTime.UtcNow
                    }, cancellationToken);
                }
            }
            else if (request.CommentId.HasValue)
            {
                var comment = await _commentRepository.GetByIdAsync(request.CommentId.Value, cancellationToken);
                if (comment != null && comment.AuthorId != request.UserId)
                {
                    await _notificationRepository.AddAsync(new Notification
                    {
                        UserId = comment.AuthorId,
                        Message = $"{request.Username} liked your comment",
                        Type = NotificationType.LikeComment,
                        RelatedId = request.CommentId.Value,
                        IsRead = false,
                        CreatedAt = DateTime.UtcNow
                    }, cancellationToken);
                }
            }
        }
        else
        {
            if (existingLike.Emoji != request.Emoji)
            {
                oldEmoji = existingLike.Emoji;
                existingLike.Emoji = request.Emoji;
                await _likeRepository.UpdateAsync(existingLike, cancellationToken);
                type = ToggleLikeType.Updated;
            }
            else
            {
                await _likeRepository.DeleteAsync(existingLike, cancellationToken);
            }
        }

        IDomainEvent? likeEvent = null;
        if (request.PostId.HasValue)
        {
            likeEvent = new PostLikeAddedEvent(existingLike, type, oldEmoji);
        }
        else if (request.CommentId.HasValue)
        {
            likeEvent = new CommentLikeAddedEvent(existingLike, type, oldEmoji);
        }

        if (likeEvent != null)
        {
            await _dispatcher.Publish(likeEvent, cancellationToken);
        }
        
        // Update User Last Active
        var user = await _userRepository.GetByIdAsync(request.UserId.GetValueOrDefault(), cancellationToken);
        if (user != null)
        {
            user.LastActiveAt = DateTime.UtcNow;
            await _userRepository.UpdateAsync(user, cancellationToken);
        }
        return true; // Added
    }
}