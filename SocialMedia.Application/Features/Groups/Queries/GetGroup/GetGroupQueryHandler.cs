namespace SocialMedia.Application;

public class GetGroupQueryHandler : IQueryHandler<GetGroupQuery, GroupDto?>
{
    private readonly IGroupRepository _groupRepository;

    public GetGroupQueryHandler(IGroupRepository groupRepository)
    {
        _groupRepository = groupRepository;
    }

    public async Task<GroupDto?> HandleAsync(GetGroupQuery query, CancellationToken cancellationToken)
    {
        var group = await _groupRepository.GetByIdAsync(query.GroupId, cancellationToken);
        if (group == null) return null;

        return new GroupDto
        {
            Id = group.Id,
            Name = group.Name,
            Description = group.Description,
            Type = group.Type,
            CreatorId = group.CreatorId,
            CreatedAt = group.CreatedAt
        };
    }
}