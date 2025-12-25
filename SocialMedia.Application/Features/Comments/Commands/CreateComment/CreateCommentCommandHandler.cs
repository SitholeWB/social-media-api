namespace SocialMedia.Application;

public class CreateCommentCommandHandler : ICommandHandler<CreateCommentCommand, Guid>
{
    private readonly ICommentRepository _commentRepository;
    private readonly IPostRepository _postRepository;
    private readonly INotificationRepository _notificationRepository;
    private readonly IDispatcher _dispatcher;
    private readonly IPostVectorService _postVectorService;

    public CreateCommentCommandHandler(
        ICommentRepository commentRepository,
        IPostRepository postRepository,
        INotificationRepository notificationRepository,
        IDispatcher dispatcher,
        IPostVectorService postVectorService)
    {
        _commentRepository = commentRepository;
        _postRepository = postRepository;
        _notificationRepository = notificationRepository;
        _dispatcher = dispatcher;
        _postVectorService = postVectorService;
    }

    public async Task<Guid> Handle(CreateCommentCommand command, CancellationToken cancellationToken)
    {
        var post = await _postRepository.GetByIdAsync(command.CreateCommentDto.PostId, cancellationToken);
        if (post == null)
        {
            throw new KeyNotFoundException($"Post with ID {command.CreateCommentDto.PostId} not found.");
        }

        var comment = command.CreateCommentDto.ToEntity();
        var createdComment = await _commentRepository.AddAsync(comment, cancellationToken);

        // Create Notification
        if (post.AuthorId != comment.AuthorId)
        {
            await _notificationRepository.AddAsync(new Notification
            {
                UserId = post.AuthorId,
                Message = $"Someone commented on your post",
                Type = NotificationType.Comment,
                RelatedId = post.Id,
                IsRead = false,
                CreatedAt = DateTime.UtcNow
            }, cancellationToken);
        }

        // Publish Event
        await _dispatcher.Publish(new CommentAddedEvent(createdComment), cancellationToken);

        // Integration with Vector Service - Treat comment as an update to post context
        // In a real scenario, we might want to concatenate or update the post embedding
        await _postVectorService.UpsertPostEmbeddingAsync(post.Id, $"{post.Content} {createdComment.Content}", cancellationToken);

        return createdComment.Id;
    }
}
