namespace SocialMedia.Application;

public class DeleteGroupCommandHandler : ICommandHandler<DeleteGroupCommand, bool>
{
    private readonly IGroupRepository _groupRepository;

    public DeleteGroupCommandHandler(IGroupRepository groupRepository)
    {
        _groupRepository = groupRepository;
    }

    public async Task<bool> Handle(DeleteGroupCommand command, CancellationToken cancellationToken)
    {
        if (DefaultConstants.DEFAULT_GROUPS.Any(x => x.Id == command.GroupId))
        {
            return false;
        }
        var group = await _groupRepository.GetByIdAsync(command.GroupId, cancellationToken);
        if (group == null)
        {
            return false;
        }
        await _groupRepository.DeleteAsync(group, cancellationToken);
        return true;
    }
}