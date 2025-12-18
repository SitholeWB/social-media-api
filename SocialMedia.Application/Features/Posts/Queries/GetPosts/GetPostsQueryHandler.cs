namespace SocialMedia.Application;

public class GetPostsQueryHandler : IQueryHandler<GetPostsQuery, PagedResult<PostDto>>
{
    private readonly IPostReadRepository _readRepository;
    private readonly IGroupRepository _groupRepository;
    private readonly IGroupMemberRepository _groupMemberRepository;

    public GetPostsQueryHandler(
        IPostReadRepository readRepository,
        IGroupRepository groupRepository,
        IGroupMemberRepository groupMemberRepository)
    {
        _readRepository = readRepository;
        _groupRepository = groupRepository;
        _groupMemberRepository = groupMemberRepository;
    }

    public async Task<PagedResult<PostDto>> Handle(GetPostsQuery request, CancellationToken cancellationToken)
    {
        var group = await _groupRepository.GetByIdAsync(request.GroupId, cancellationToken);
        if (group != null && group.Type == GroupType.Private)
        {
            if (!request.UserId.HasValue)
            {
                throw new UnauthorizedAccessException("You must be logged in and a member of this group to view posts.");
            }

            var isMember = await _groupMemberRepository.ExistsAsync(group.Id, request.UserId.Value, cancellationToken);
            if (!isMember)
            {
                throw new UnauthorizedAccessException("You must be a member of this group to view posts.");
            }
        }

        var posts = request.SortBy == PostSortBy.Trending
            ? await _readRepository.GetTrendingAsync(request.GroupId, request.PageNumber, request.PageSize, cancellationToken)
            : await _readRepository.GetLatestAsync(request.GroupId, request.PageNumber, request.PageSize, cancellationToken);

        var dtos = posts.Select(p => new PostDto
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
            Tags = p.Tags,
            Reactions = p.Reactions,
            StatusFullScreen = p.StatusFullScreen,
        }).ToList();

        // Get total count for pagination
        var totalCount = await _readRepository.GetTotalCountAsync(request.GroupId, cancellationToken);

        return new PagedResult<PostDto>(dtos, totalCount, request.PageNumber, request.PageSize);
    }
}