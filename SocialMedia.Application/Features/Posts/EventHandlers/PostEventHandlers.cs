namespace SocialMedia.Application;

public class PostEventHandlers :
    IEventHandler<PostCreatedEvent>,
    IEventHandler<LikeAddedEvent>,
    IEventHandler<CommentAddedEvent>
{
    private readonly IPostReadRepository _readRepository;
    private readonly ICommentReadRepository _commentReadRepository;
    private readonly IUserRepository _userRepository; // To get Author Name
    private readonly IPostRepository _postRepository; // To get Post details for Like/Comment events

    public PostEventHandlers(
        IPostReadRepository readRepository,
        ICommentReadRepository commentReadRepository,
        IUserRepository userRepository,
        IPostRepository postRepository)
    {
        _readRepository = readRepository;
        _commentReadRepository = commentReadRepository;
        _userRepository = userRepository;
        _postRepository = postRepository;
    }

    public async Task Handle(PostCreatedEvent notification, CancellationToken cancellationToken)
    {
        if (notification.Post == null)
        {
            throw new ArgumentNullException(nameof(notification.Post), "Post in PostCreatedEvent is null. This might be due to JSON deserialization issues.");
        }

        var author = await _userRepository.GetByIdAsync(notification.Post.AuthorId, cancellationToken);

        var readModel = new PostReadModel
        {
            Id = notification.Post.Id,
            Title = notification.Post.Title,
            Content = notification.Post.Content,
            AuthorId = notification.Post.AuthorId,
            AuthorName = author?.Username ?? "Unknown",
            CreatedAt = notification.Post.CreatedAt,
            FileUrl = notification.Post.File?.Url,

            Stats = new PostStats
            {
                LikeCount = 0,
                CommentCount = 0,
                TrendingScore = 0
            },
            GroupId = notification.Post.Groups.FirstOrDefault()?.Id, // Simplified for now
            GroupName = notification.Post.Groups.FirstOrDefault()?.Name
        };

        await _readRepository.AddAsync(readModel, cancellationToken);
    }

    public async Task Handle(LikeAddedEvent notification, CancellationToken cancellationToken)
    {
        if (notification.Like == null)
        {
            throw new ArgumentNullException(nameof(notification.Like), "Like in LikeAddedEvent is null. This might be due to JSON deserialization issues.");
        }

        var user = await _userRepository.GetByIdAsync(notification.Like.UserId, cancellationToken);
        var reaction = new ReactionReadDto
        {
            UserId = notification.Like.UserId,
            UserName = user?.Username ?? "Unknown",
            UserProfilePicUrl = null, // TODO: Add ProfilePic to User entity
            Emoji = notification.Like.Emoji
        };

        if (notification.Like.PostId.HasValue)
        {
            var post = await _readRepository.GetByIdAsync(notification.Like.PostId.Value, cancellationToken);
            if (post != null)
            {
                post.Stats.LikeCount++;
                post.Reactions.Add(reaction);
                post.UpdateTrendingScore();
                await _readRepository.UpdateAsync(post, cancellationToken);
            }
        }
        else if (notification.Like.CommentId.HasValue)
        {
            var comment = await _commentReadRepository.GetByIdAsync(notification.Like.CommentId.Value, cancellationToken);
            if (comment != null)
            {
                comment.Stats.LikeCount++;
                comment.Reactions.Add(reaction);
                await _commentReadRepository.UpdateAsync(comment, cancellationToken);

                // Update TopComments in PostReadModel if present
                var post = await _readRepository.GetByIdAsync(comment.PostId, cancellationToken);
                if (post != null)
                {
                    var topComment = post.TopComments.FirstOrDefault(c => c.Id == comment.Id);
                    if (topComment != null)
                    {
                        topComment.LikeCount++;
                        topComment.Reactions.Add(reaction);
                        await _readRepository.UpdateAsync(post, cancellationToken);
                    }
                }
            }
        }
    }

    public async Task Handle(CommentAddedEvent notification, CancellationToken cancellationToken)
    {
        if (notification.Comment == null)
        {
            throw new ArgumentNullException(nameof(notification.Comment), "Comment in CommentAddedEvent is null. This might be due to JSON deserialization issues.");
        }

        var post = await _readRepository.GetByIdAsync(notification.Comment.PostId, cancellationToken);
        if (post != null)
        {
            var author = await _userRepository.GetByIdAsync(notification.Comment.AuthorId, cancellationToken);

            var commentDto = new CommentReadDto
            {
                Id = notification.Comment.Id,
                Content = notification.Comment.Content,
                AuthorId = notification.Comment.AuthorId,
                AuthorName = author?.Username ?? "Unknown",
                AuthorProfilePicUrl = null,
                CreatedAt = notification.Comment.CreatedAt,
                LikeCount = 0
            };

            var commentReadModel = new CommentReadModel
            {
                Id = notification.Comment.Id,
                PostId = notification.Comment.PostId,
                Content = notification.Comment.Content,
                AuthorId = notification.Comment.AuthorId,
                AuthorName = author?.Username ?? "Unknown",
                AuthorProfilePicUrl = null,
                CreatedAt = notification.Comment.CreatedAt,
                Stats = new CommentStats { LikeCount = 0 }
            };

            await _commentReadRepository.AddAsync(commentReadModel, cancellationToken);

            post.Stats.CommentCount++;

            // Add to TopComments (keep max 30)
            post.TopComments.Add(commentDto);
            if (post.TopComments.Count > 30)
            {
                // Remove oldest or least relevant? Requirement says "more reactions or new".
                // For now, let's just keep the latest 30.
                var oldest = post.TopComments.OrderBy(c => c.CreatedAt).First();
                post.TopComments.Remove(oldest);
            }

            post.UpdateTrendingScore();
            await _readRepository.UpdateAsync(post, cancellationToken);
        }
    }
}
