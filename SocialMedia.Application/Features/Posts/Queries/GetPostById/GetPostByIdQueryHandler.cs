namespace SocialMedia.Application;

public class GetPostByIdQueryHandler : IQueryHandler<GetPostByIdQuery, PostDto?>
{
    private readonly IPostRepository _postRepository;

    public GetPostByIdQueryHandler(IPostRepository postRepository)
    {
        _postRepository = postRepository;
    }

    public async Task<PostDto?> Handle(GetPostByIdQuery request, CancellationToken cancellationToken)
    {
        var post = await _postRepository.GetByIdAsync(request.Id, cancellationToken);
        return post?.ToDto();
    }
}
