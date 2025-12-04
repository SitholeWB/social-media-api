using SocialMedia.Domain;

namespace SocialMedia.Application;

public class GetGroupsQueryHandler : IQueryHandler<GetGroupsQuery, PagedResult<GroupDto>>
{
    private readonly IGroupRepository _groupRepository;

    public GetGroupsQueryHandler(IGroupRepository groupRepository)
    {
        _groupRepository = groupRepository;
    }

    public async Task<PagedResult<GroupDto>> Handle(GetGroupsQuery query, CancellationToken cancellationToken)
    {
        var (items, totalCount) = await _groupRepository.GetGroupsPagedAsync(query.PageNumber, query.PageSize, cancellationToken);

        var dtos = items.Select(g => new GroupDto
        {
            Id = g.Id,
            Name = g.Name,
            Description = g.Description,
            IsPublic = g.IsPublic,
            IsAutoAdd = g.IsAutoAdd,
            CreatorId = Guid.Empty, // Group entity doesn't have CreatorId
            CreatedAt = g.CreatedAt
        }).ToList();

        return new PagedResult<GroupDto>(dtos, totalCount, query.PageNumber, query.PageSize);
    }
}