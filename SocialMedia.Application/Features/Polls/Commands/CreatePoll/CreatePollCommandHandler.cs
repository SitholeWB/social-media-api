namespace SocialMedia.Application;

public class CreatePollCommandHandler : ICommandHandler<CreatePollCommand, Guid>
{
    private readonly IPollRepository _pollRepository;

    public CreatePollCommandHandler(IPollRepository pollRepository)
    {
        _pollRepository = pollRepository;
    }

    public async Task<Guid> Handle(CreatePollCommand command, System.Threading.CancellationToken cancellationToken)
    {
        var poll = new Poll
        {
            Id = Guid.NewGuid(),
            Question = command.Question,
            ExpiresAt = command.ExpiresAt,
            CreatorId = command.CreatorId,
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
