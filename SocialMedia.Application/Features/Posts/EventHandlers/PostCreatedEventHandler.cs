namespace SocialMedia.Application;

public class PostCreatedEventHandler :
    IEventHandler<PostCreatedEvent>
{
    private readonly IPostReadRepository _readRepository;
    private readonly IUserRepository _userRepository; // To get Author Name

    public PostCreatedEventHandler(
        IPostReadRepository readRepository,
        IUserRepository userRepository)
    {
        _readRepository = readRepository;
        _userRepository = userRepository;
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
            AuthorName = author?.GetFullName() ?? "Unknown",
            CreatedAt = notification.Post.CreatedAt,
            Media = notification.Post.Media,
            AdminTags = notification.Post.AdminTags,
            StatusFullScreen = notification.Post.StatusFullScreen,
            Reactions = new List<ReactionReadDto>(),
            TopComments = new List<CommentReadDto>(),
            Tags = notification.Post.Tags,
            Stats = new PostStatsDto
            {
                LikeCount = 0,
                CommentCount = 0,
                TrendingScore = 0
            },
            GroupId = notification.Post.GroupId,
            GroupName = notification.Post.Group?.Name
        };

        await _readRepository.AddAsync(readModel, cancellationToken);
    }
}