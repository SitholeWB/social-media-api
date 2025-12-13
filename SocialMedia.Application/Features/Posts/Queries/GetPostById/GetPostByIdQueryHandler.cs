namespace SocialMedia.Application;

public class GetPostByIdQueryHandler : IQueryHandler<GetPostByIdQuery, PostDto?>
{
    private readonly IPostReadRepository _postRepository;

    public GetPostByIdQueryHandler(IPostReadRepository postRepository)
    {
        _postRepository = postRepository;
    }

    public async Task<PostDto?> Handle(GetPostByIdQuery request, CancellationToken cancellationToken)
    {
        var p = await _postRepository.GetByIdAsync(request.Id, cancellationToken);
        p ??= new PostReadModel();
        return new PostDto
        {
            Id = p.Id,
            Title = p.Title,
            Content = p.Content,
            AuthorId = p.AuthorId,
            AuthorName = p.AuthorName,
            CreatedAt = p.CreatedAt,
            FileUrl = p.FileUrl,
            LikeCount = p.Stats.LikeCount,
            CommentCount = p.Stats.CommentCount,
            AdminTags = p.AdminTags,
            Tags = p.Tags
        };
    }
}