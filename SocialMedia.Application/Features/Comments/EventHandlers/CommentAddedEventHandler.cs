namespace SocialMedia.Application;

public class CommentAddedEventHandler :
    IEventHandler<CommentAddedEvent>
{
    private readonly IPostReadRepository _readRepository;
    private readonly ICommentReadRepository _commentReadRepository;
    private readonly IUserRepository _userRepository; // To get Author Name

    public CommentAddedEventHandler(
        IPostReadRepository readRepository,
        ICommentReadRepository commentReadRepository,
        IUserRepository userRepository)
    {
        _readRepository = readRepository;
        _commentReadRepository = commentReadRepository;
        _userRepository = userRepository;
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
            var createdBy = notification.Comment.CreatedBy;
            if (string.IsNullOrWhiteSpace(createdBy))
            {
                var author = await _userRepository.GetByIdAsync(notification.Comment.AuthorId, cancellationToken);
                createdBy = author?.GetFullName() ?? "Unknown";
            }
            var commentDto = new CommentReadDto
            {
                CommentId = notification.Comment.Id,
                Content = notification.Comment.Content,
                Title = notification.Comment.Title,
                AuthorId = notification.Comment.AuthorId,
                AuthorName = createdBy,
                AuthorProfilePicUrl = null,
                Media = notification.Comment.Media,
                CreatedAt = notification.Comment.CreatedAt,
                LikeCount = 0,
                Reactions = new List<ReactionReadDto>(),
                AdminTags = notification.Comment.AdminTags,
                Tags = notification.Comment.Tags
            };

            var commentReadModel = new CommentReadModel
            {
                Id = notification.Comment.Id,
                PostId = notification.Comment.PostId,
                Content = notification.Comment.Content,
                Title = notification.Comment.Title,
                AuthorId = notification.Comment.AuthorId,
                AuthorName = createdBy,
                AuthorProfilePicUrl = null,
                Media = notification.Comment.Media,
                CreatedAt = notification.Comment.CreatedAt,
                Stats = new CommentStatsDto { LikeCount = 0 },
                Reactions = new List<ReactionReadDto>(),
                Tags = notification.Comment.Tags,
                AdminTags = notification.Comment.AdminTags
            };

            await _commentReadRepository.AddAsync(commentReadModel, cancellationToken);

            post.Stats.CommentCount++;

            // Add to TopComments (keep max 30)
            post.TopComments.Add(commentDto);
            if (post.TopComments.Count > 30)
            {
                // Remove oldest or least relevant? Requirement says "more reactions or new". For
                // now, let's just keep the latest 30.
                var oldest = post.TopComments.OrderBy(c => c.CreatedAt).First();
                post.TopComments.Remove(oldest);
            }

            post.UpdateTrendingScore();
            await _readRepository.UpdateAsync(post, cancellationToken);
        }
    }
}