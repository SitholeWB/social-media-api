namespace SocialMedia.Application;

public class PostDeletedEventHandler :
    IEventHandler<PostDeletedEvent>
{
    private readonly IPostService _postService;

    public PostDeletedEventHandler(IPostService postService)
    {
        _postService = postService;
    }

    public async Task Handle(PostDeletedEvent notification, CancellationToken cancellationToken)
    {
        if (notification.Post == null)
        {
            throw new ArgumentNullException(nameof(notification.Post));
        }

        await _postService.DeletePostAsync(notification.Post.Id, cancellationToken);
    }
}