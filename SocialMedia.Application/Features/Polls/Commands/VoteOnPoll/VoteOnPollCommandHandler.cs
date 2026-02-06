namespace SocialMedia.Application;

public class VoteOnPollCommandHandler : ICommandHandler<VoteOnPollCommand, bool>
{
    private readonly IPollRepository _pollRepository;
    private readonly IRepository<Vote> _voteRepository;
    private readonly IBlockchainService _blockchainService;
    private readonly IGroupRepository _groupRepository;
    private readonly IGroupMemberRepository _groupMemberRepository;
    private readonly IDispatcher _dispatcher;

    public VoteOnPollCommandHandler(
        IPollRepository pollRepository,
        IRepository<Vote> voteRepository,
        IBlockchainService blockchainService,
        IGroupRepository groupRepository,
        IGroupMemberRepository groupMemberRepository,
        IDispatcher dispatcher)
    {
        _pollRepository = pollRepository;
        _voteRepository = voteRepository;
        _blockchainService = blockchainService;
        _groupRepository = groupRepository;
        _groupMemberRepository = groupMemberRepository;
        _dispatcher = dispatcher;
    }

    public async Task<bool> HandleAsync(VoteOnPollCommand command, System.Threading.CancellationToken cancellationToken)
    {
        var poll = await _pollRepository.GetByIdAsync(command.PollId, cancellationToken);
        if (poll == null || !poll.IsActive || (poll.ExpiresAt.HasValue && poll.ExpiresAt < DateTime.UtcNow))
        {
            return false;
        }

        // Access Check: If you are allowed to create new post then you can vote.
        var group = await _groupRepository.GetByIdAsync(poll.GroupId, cancellationToken);
        if (group != null && group.Type != GroupType.Everyone)
        {
            var isMember = await _groupMemberRepository.ExistsAsync(poll.GroupId, command.UserId, cancellationToken);
            if (!isMember)
            {
                throw new UnauthorizedAccessException("You must be a member of this group to vote.");
            }
        }

        var hasVoted = await _pollRepository.HasUserVotedAsync(command.PollId, command.UserId, cancellationToken);
        if (hasVoted)
        {
            throw new InvalidOperationException("User has already voted on this poll.");
        }

        // 1. Record that the user has voted (Anonymized duplicate prevention)
        var voteRecord = new PollVoteRecord
        {
            Id = Guid.NewGuid(),
            PollId = command.PollId,
            UserId = command.UserId,
            CreatedAt = DateTime.UtcNow
        };
        await _pollRepository.AddVoteRecordAsync(voteRecord, cancellationToken);

        // 2. Record the actual vote selection
        var vote = new Vote
        {
            Id = Guid.NewGuid(),
            PollOptionId = command.PollOptionId,
            UserId = poll.IsAnonymous ? null : command.UserId, // SEPARATION: selection is anonymous if requested
            CreatedAt = DateTime.UtcNow
        };

        await _voteRepository.AddAsync(vote, cancellationToken);

        // 3. Update blockchain (Immutable audit log) For anonymous polls, we record Guid.Empty as
        // UserId to maintain total anonymity on the ledger
        var userIdForBlockchain = poll.IsAnonymous ? Guid.Empty : command.UserId;
        await _blockchainService.AddVoteAsync(vote.Id, userIdForBlockchain, command.PollOptionId, cancellationToken);

        // 4. Publish Event
        await _dispatcher.PublishAsync(new PollVotedEvent(command.PollId, command.PollOptionId, command.UserId), cancellationToken);

        return true;
    }
}