using Microsoft.EntityFrameworkCore;

namespace SocialMedia.Application;

public class UpdatePollCommandHandler : ICommandHandler<UpdatePollCommand, bool>
{
    private readonly SocialMediaDbContext _context;

    public UpdatePollCommandHandler(SocialMediaDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(UpdatePollCommand command, CancellationToken cancellationToken)
    {
        var poll = await _context.Polls.FirstOrDefaultAsync(p => p.Id == command.PollId, cancellationToken);
        if (poll == null)
        {
            return false;
        }

        poll.Update(command.Question, command.IsActive, command.ExpiresAt);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
