namespace SocialMedia.Application;

public class CreatePostCommandHandler : ICommandHandler<CreatePostCommand, Guid>
{
    private readonly IPostRepository _postRepository;
    private readonly IGroupRepository _groupRepository;
    private readonly IGroupMemberRepository _groupMemberRepository;
    private readonly IDispatcher _dispatcher;

    public CreatePostCommandHandler(
        IPostRepository postRepository,
        IGroupRepository groupRepository,
        IGroupMemberRepository groupMemberRepository,
        IDispatcher dispatcher)
    {
        _postRepository = postRepository;
        _groupRepository = groupRepository;
        _groupMemberRepository = groupMemberRepository;
        _dispatcher = dispatcher;
    }

    public async Task<Guid> HandleAsync(CreatePostCommand request, CancellationToken cancellationToken)
    {
        if (request.PostDto.GroupId != Guid.Empty)
        {
            var group = await _groupRepository.GetByIdAsync(request.PostDto.GroupId, cancellationToken);
            if (group != null && group.Type != GroupType.Everyone)
            {
                var isMember = await _groupMemberRepository.ExistsAsync(group.Id, request.PostDto.AuthorId, cancellationToken);
                if (!isMember)
                {
                    throw new UnauthorizedAccessException("You must be a member of this group to post.");
                }
            }
        }

        var post = request.PostDto.ToEntity();
        var createdPost = await _postRepository.AddAsync(post, cancellationToken);

        // Reload with File navigation property for the event
        var postWithFile = await _postRepository.GetByIdAsync(createdPost.Id, cancellationToken);

        await _dispatcher.PublishAsync(new PostCreatedEvent(postWithFile ?? createdPost), cancellationToken);

        return createdPost.Id;
    }
}