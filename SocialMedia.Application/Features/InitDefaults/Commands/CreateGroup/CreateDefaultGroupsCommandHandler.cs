using System.Text;

namespace SocialMedia.Application;

public class CreateDefaultGroupsCommandHandler : ICommandHandler<CreateDefaultGroupsCommand, string>
{
    private readonly IGroupRepository _groupRepository;

    public CreateDefaultGroupsCommandHandler(IGroupRepository groupRepository)
    {
        _groupRepository = groupRepository;
    }

    public async Task<string> Handle(CreateDefaultGroupsCommand request, CancellationToken cancellationToken)
    {
        var sb = new StringBuilder();
        foreach (var defaultGroup in request.DefaultGroups)
        {
            var existingGroup = await _groupRepository.GetByIdAsync(defaultGroup.Id, cancellationToken);
            if (existingGroup == null)
            {
                var group = new Group
                {
                    Id = defaultGroup.Id,
                    Name = defaultGroup.Name,
                    Description = defaultGroup.Description,
                    IsPublic = defaultGroup.IsPublic,
                    IsAutoAdd = defaultGroup.IsAutoAdd,
                    CreatedAt = DateTime.UtcNow,
                    Members = new List<GroupMember>(),
                    Posts = new List<Post>()
                };
                await _groupRepository.AddAsync(group, cancellationToken);
                sb.AppendLine($"Created group: {group.Name} (ID: {group.Id})");
            }
            else
            {
                sb.AppendLine($"Already exist group: {existingGroup.Name} (ID: {existingGroup.Id})");
            }
        }

        return sb.ToString();
    }
}