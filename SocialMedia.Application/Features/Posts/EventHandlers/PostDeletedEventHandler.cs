namespace SocialMedia.Application;

public class PostDeletedEventHandler :
    IEventHandler<PostDeletedEvent>
{
    private readonly IPostReadRepository _readRepository;
    private readonly ICommentReadRepository _commentReadRepository;

    public PostDeletedEventHandler(
        IPostReadRepository readRepository,
        ICommentReadRepository commentReadRepository)
    {
        _readRepository = readRepository;
        _commentReadRepository = commentReadRepository;
    }

    public async Task Handle(PostDeletedEvent notification, CancellationToken cancellationToken)
    {
        if (notification.Post == null)
        {
            throw new ArgumentNullException(nameof(notification.Post));
        }

        var post = await _readRepository.GetByIdAsync(notification.Post.Id, cancellationToken);
        if (post != null)
        {
            await _readRepository.DeleteByIdAsync(post.Id, cancellationToken);
            var comments = new List<CommentReadModel>();
            do
            {
                comments = await _commentReadRepository.GetByPostIdAsync(post.Id, 1, 100, cancellationToken);
                foreach (var comment in comments)
                {
                    await _commentReadRepository.DeleteByIdAsync(comment.Id, cancellationToken);
                }
            } while (comments.Count > 0);
        }
    }
}