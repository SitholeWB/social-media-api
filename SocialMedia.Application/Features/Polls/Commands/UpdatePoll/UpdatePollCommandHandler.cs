using SocialMedia.Domain;

namespace SocialMedia.Application;

public class UpdatePollCommandHandler : ICommandHandler<UpdatePollCommand, bool>
{
    private readonly IPollRepository _pollRepository;

    public UpdatePollCommandHandler(IPollRepository pollRepository)
    {
        _pollRepository = pollRepository;
    }

    public async Task<bool> Handle(UpdatePollCommand command, CancellationToken cancellationToken)
    {
        var poll = await _pollRepository.GetByIdAsync(command.PollId, cancellationToken);
        if (poll == null)
        {
            return false;
        }

        poll.Update(command.Question, command.IsActive, command.ExpiresAt);
        await _pollRepository.UpdateAsync(poll, cancellationToken);
        return true;
    }
}
