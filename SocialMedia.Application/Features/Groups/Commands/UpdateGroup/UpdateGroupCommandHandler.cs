namespace SocialMedia.Application;

public class UpdateGroupCommandHandler : ICommandHandler<UpdateGroupCommand, bool>
{
    private readonly IGroupRepository _groupRepository;

    public UpdateGroupCommandHandler(IGroupRepository groupRepository)
    {
        _groupRepository = groupRepository;
    }

    public async Task<bool> HandleAsync(UpdateGroupCommand command, CancellationToken cancellationToken)
    {
        var group = await _groupRepository.GetByIdAsync(command.GroupId, cancellationToken);
        if (group == null)
        {
            return false;
        }
        group.Update(command.Name, command.Description, command.Type);
        await _groupRepository.UpdateAsync(group, cancellationToken);
        return true;
    }
}