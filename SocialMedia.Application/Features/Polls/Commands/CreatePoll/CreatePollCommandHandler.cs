namespace SocialMedia.Application;

public class CreatePollCommandHandler : ICommandHandler<CreatePollCommand, Guid>
{
    private readonly IPollRepository _pollRepository;
    private readonly IGroupRepository _groupRepository;
    private readonly IGroupMemberRepository _groupMemberRepository;

    public CreatePollCommandHandler(IPollRepository pollRepository, IGroupRepository groupRepository, IGroupMemberRepository groupMemberRepository)
    {
        _pollRepository = pollRepository;
        _groupRepository = groupRepository;
        _groupMemberRepository = groupMemberRepository;
    }

    public async Task<Guid> HandleAsync(CreatePollCommand command, System.Threading.CancellationToken cancellationToken)
    {
        // Access Check
        var group = await _groupRepository.GetByIdAsync(command.GroupId, cancellationToken);
        if (group == null)
        {
            throw new InvalidOperationException("Group not found.");
        }

        if (group.Type != GroupType.Everyone)
        {
            var isMember = await _groupMemberRepository.ExistsAsync(command.GroupId, command.CreatorId, cancellationToken);
            if (!isMember)
            {
                throw new UnauthorizedAccessException("You must be a member of this group to create a poll.");
            }
        }

        var poll = new Poll
        {
            Id = Guid.NewGuid(),
            Question = command.Question,
            ExpiresAt = command.ExpiresAt,
            CreatorId = command.CreatorId,
            IsAnonymous = command.IsAnonymous,
            GroupId = command.GroupId,
            IsActive = true,
            CreatedAt = DateTime.UtcNow
        };

        foreach (var optionText in command.Options)
        {
            poll.Options.Add(new PollOption
            {
                Id = Guid.NewGuid(),
                Text = optionText,
                PollId = poll.Id,
                CreatedAt = DateTime.UtcNow
            });
        }

        await _pollRepository.AddAsync(poll, cancellationToken);
        return poll.Id;
    }
}