namespace SocialMedia.Application;

public class DeletePostCommandHandler : ICommandHandler<DeletePostCommand, bool>
{
    private readonly IPostRepository _postRepository;
    private readonly IDispatcher _dispatcher;

    public DeletePostCommandHandler(IPostRepository postRepository, IDispatcher dispatcher)
    {
        _postRepository = postRepository;
        _dispatcher = dispatcher;
    }

    public async Task<bool> HandleAsync(DeletePostCommand request, CancellationToken cancellationToken)
    {
        var post = await _postRepository.GetByIdAsync(request.PostId, cancellationToken);
        if (post == null || post.AuthorId != request.UserId)
        {
            return false;
        }
        post.IsDeleted = true;
        await _postRepository.UpdateAsync(post, cancellationToken);
        await _dispatcher.PublishAsync(new PostCreatedEvent(post), cancellationToken);

        return true;
    }
}