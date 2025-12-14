namespace SocialMedia.Application;

public class CommentDeletedEventHandler :
    IEventHandler<CommentDeletedEvent>
{
    private readonly IPostReadRepository _readRepository;
    private readonly ICommentReadRepository _commentReadRepository;

    public CommentDeletedEventHandler(
        IPostReadRepository readRepository,
        ICommentReadRepository commentReadRepository)
    {
        _readRepository = readRepository;
        _commentReadRepository = commentReadRepository;
    }

    public async Task Handle(CommentDeletedEvent notification, CancellationToken cancellationToken)
    {
        if (notification.Comment == null)
        {
            throw new ArgumentNullException(nameof(notification.Comment), "Comment in CommentAddedEvent is null. This might be due to JSON deserialization issues.");
        }

        var comment = await _commentReadRepository.GetByIdAsync(notification.Comment.Id, cancellationToken);
        if (comment != null)
        {
            await _commentReadRepository.DeleteByIdAsync(comment.Id, cancellationToken);
            var post = await _readRepository.GetByIdAsync(comment.PostId, cancellationToken);
            if (post != null && post.TopComments.Any(x => x.CommentId == comment.Id))
            {
                post.TopComments = post.TopComments.Where(x => x.CommentId != comment.Id).ToList();
                await _readRepository.UpdateAsync(post, cancellationToken);
            }
        }
    }
}