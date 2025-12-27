namespace SocialMedia.Application;

public class GetPollQueryHandler : IQueryHandler<GetPollQuery, PollDto?>
{
    private readonly IPollRepository _pollRepository;
    private readonly IGroupRepository _groupRepository;
    private readonly IGroupMemberRepository _groupMemberRepository;
    private readonly IUserActivityRepository _userActivityRepository;

    public GetPollQueryHandler(
        IPollRepository pollRepository,
        IGroupRepository groupRepository,
        IGroupMemberRepository groupMemberRepository,
        IUserActivityRepository userActivityRepository)
    {
        _pollRepository = pollRepository;
        _groupRepository = groupRepository;
        _groupMemberRepository = groupMemberRepository;
        _userActivityRepository = userActivityRepository;
    }

    public async Task<PollDto?> Handle(GetPollQuery query, CancellationToken cancellationToken)
    {
        var poll = await _pollRepository.GetByIdAsync(query.PollId, cancellationToken);
        if (poll == null)
        {
            return null;
        }

        // Visibility Check
        var group = await _groupRepository.GetByIdAsync(poll.GroupId, cancellationToken);
        if (group != null && group.Type == GroupType.Private)
        {
            if (!query.UserId.HasValue)
            {
                throw new UnauthorizedAccessException("You must be logged in and a member of this group to view this poll.");
            }

            var isMember = await _groupMemberRepository.ExistsAsync(poll.GroupId, query.UserId.Value, cancellationToken);
            if (!isMember)
            {
                throw new UnauthorizedAccessException("You must be a member of this group to view this poll.");
            }
        }

        UserActivity? userActivity = null;
        if (query.UserId.HasValue)
        {
            userActivity = await _userActivityRepository.GetByUserIdAsync(query.UserId.Value, cancellationToken);
        }

        return poll.ToDto(userActivity);
    }
}