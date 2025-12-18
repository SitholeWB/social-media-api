namespace SocialMedia.Application;

public class GetActivePollsQueryHandler : IQueryHandler<GetActivePollsQuery, PagedResult<PollDto>>
{
    private readonly IPollRepository _pollRepository;
    private readonly IGroupRepository _groupRepository;
    private readonly IGroupMemberRepository _groupMemberRepository;

    public GetActivePollsQueryHandler(
        IPollRepository pollRepository,
        IGroupRepository groupRepository,
        IGroupMemberRepository groupMemberRepository)
    {
        _pollRepository = pollRepository;
        _groupRepository = groupRepository;
        _groupMemberRepository = groupMemberRepository;
    }

    public async Task<PagedResult<PollDto>> Handle(GetActivePollsQuery query, CancellationToken cancellationToken)
    {
        // Visibility Check
        var group = await _groupRepository.GetByIdAsync(query.GroupId, cancellationToken);
        if (group != null && group.Type == GroupType.Private)
        {
            if (!query.UserId.HasValue)
            {
                throw new UnauthorizedAccessException("You must be logged in and a member of this group to view polls.");
            }

            var isMember = await _groupMemberRepository.ExistsAsync(query.GroupId, query.UserId.Value, cancellationToken);
            if (!isMember)
            {
                throw new UnauthorizedAccessException("You must be a member of this group to view polls.");
            }
        }

        var (polls, totalCount) = await _pollRepository.GetActivePollsPagedAsync(query.GroupId, query.PageNumber, query.PageSize, cancellationToken);
        var dtos = polls.Select(p => p.ToDto()).ToList();
        return new PagedResult<PollDto>(dtos, totalCount, query.PageNumber, query.PageSize);
    }
}
