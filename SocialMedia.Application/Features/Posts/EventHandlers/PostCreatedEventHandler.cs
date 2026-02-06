using Microsoft.Extensions.Configuration;

namespace SocialMedia.Application;

public class PostCreatedEventHandler :
    IEventHandler<PostCreatedEvent>
{
    private readonly IPostReadRepository _readRepository;
    private readonly IUserRepository _userRepository; // To get Author Name
    private readonly IPostVectorService _postVectorService;
    private readonly IConfiguration _configuration;

    public PostCreatedEventHandler(
        IPostReadRepository readRepository,
        IUserRepository userRepository,
        IPostVectorService postVectorService,
        IConfiguration configuration)
    {
        _readRepository = readRepository;
        _userRepository = userRepository;
        _postVectorService = postVectorService;
        _configuration = configuration;
    }

    public async Task HandleAsync(PostCreatedEvent notification, CancellationToken cancellationToken)
    {
        if (notification.Post == null)
        {
            throw new ArgumentNullException(nameof(notification.Post), "Post in PostCreatedEvent is null. This might be due to JSON deserialization issues.");
        }
        var createdBy = notification.Post.CreatedBy;
        if (string.IsNullOrWhiteSpace(createdBy))
        {
            var author = await _userRepository.GetByIdAsync(notification.Post.AuthorId, cancellationToken);
            createdBy = author?.GetFullName() ?? "Unknown";
        }
        var profilesHost = _configuration.GetValue<string>("ProfilesHost") ?? "";

        var readModel = new PostReadModel
        {
            Id = notification.Post.Id,
            Title = notification.Post.Title,
            Content = notification.Post.Content,
            AuthorId = notification.Post.AuthorId,
            AuthorName = createdBy,
            AuthorProfilePicUrl = string.IsNullOrWhiteSpace(profilesHost) ? "" : $"{profilesHost}/api/db1/files/{notification.Post.AuthorId}",
            CreatedAt = notification.Post.CreatedAt,
            Media = notification.Post.Media,
            AdminTags = notification.Post.AdminTags,
            StatusFullScreen = notification.Post.StatusFullScreen,
            Reactions = new List<ReactionReadDto>(),
            TopComments = new List<CommentReadDto>(),
            Tags = notification.Post.Tags,
            ReactionCount = 0,
            CommentCount = 0,
            TrendingScore = 0,
            GroupId = notification.Post.GroupId,
            GroupName = notification.Post.Group?.Name
        };

        await _readRepository.AddAsync(readModel, cancellationToken);
        await _postVectorService.UpsertPostEmbeddingAsync(notification.Post.Id, $"{notification.Post.Content}", cancellationToken);
    }
}