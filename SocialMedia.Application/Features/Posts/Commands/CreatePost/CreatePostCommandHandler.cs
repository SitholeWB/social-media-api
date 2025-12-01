namespace SocialMedia.Application;

public class CreatePostCommandHandler : ICommandHandler<CreatePostCommand, Guid>
{
    private readonly IPostRepository _postRepository;
    private readonly IDispatcher _dispatcher;

    public CreatePostCommandHandler(IPostRepository postRepository, IDispatcher dispatcher)
    {
        _postRepository = postRepository;
        _dispatcher = dispatcher;
    }

    public async Task<Guid> Handle(CreatePostCommand request, CancellationToken cancellationToken)
    {
        var post = request.PostDto.ToEntity();
        var createdPost = await _postRepository.AddAsync(post, cancellationToken);

        // Reload with File navigation property for the event
        var postWithFile = await _postRepository.GetByIdAsync(createdPost.Id, cancellationToken);

        await _dispatcher.Publish(new PostCreatedEvent(postWithFile ?? createdPost), cancellationToken);

        return createdPost.Id;
    }
}
