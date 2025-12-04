using Microsoft.EntityFrameworkCore;

namespace SocialMedia.Application;

public class UpdateGroupCommandHandler : ICommandHandler<UpdateGroupCommand, bool>
{
    private readonly SocialMediaDbContext _context;

    public UpdateGroupCommandHandler(SocialMediaDbContext context)
    {
        _context = context;
    }

    public async Task<bool> Handle(UpdateGroupCommand command, CancellationToken cancellationToken)
    {
        var group = await _context.Groups.FirstOrDefaultAsync(g => g.Id == command.GroupId, cancellationToken);
        if (group == null)
        {
            return false;
        }

        group.Update(command.Name, command.Description, command.IsPublic, command.IsAutoAdd);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
}
