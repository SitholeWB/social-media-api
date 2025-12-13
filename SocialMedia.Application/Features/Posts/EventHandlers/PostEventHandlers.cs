namespace SocialMedia.Application;

public class PostEventHandlers :
    IEventHandler<PostCreatedEvent>,
    IEventHandler<PostLikeAddedEvent>
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
            AdminTags = notification.Post.AdminTags,
            Reactions = new List<ReactionReadDto>(),
            TopComments = new List<CommentReadDto>(),
            Tags = notification.Post.Tags,
            Stats = new PostStatsDto
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

    public async Task Handle(PostLikeAddedEvent notification, CancellationToken cancellationToken)
    {
        if (notification.Like == null)
        {
            throw new ArgumentNullException(nameof(notification.Like), "Like in LikeAddedEvent is null. This might be due to JSON deserialization issues.");
        }

        if (notification.Like.PostId.HasValue)
        {
            var post = await _readRepository.GetByIdAsync(notification.Like.PostId.Value, cancellationToken);
            if (post != null)
            {
                var reaction = post.Reactions.FirstOrDefault(r => r.Emoji == notification.Like.Emoji);
                if (reaction != null)
                {
                    if (notification.ToggleLikeType == ToggleLikeType.Removed)
                    {
                        reaction.Count--;
                        post.Stats.LikeCount--;
                    }
                }
                else
                {
                    post.Reactions.Add(new ReactionReadDto
                    {
                        Emoji = notification.Like.Emoji,
                        Count = 1
                    });
                    post.Stats.LikeCount++;
                }
                post.Reactions = [.. post.Reactions.Where(r => r.Count > 0)];
                post.UpdateTrendingScore();
                await _readRepository.UpdateAsync(post, cancellationToken);
            }
        }
    }
}