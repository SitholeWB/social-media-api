namespace SocialMedia.Domain;

public class Group : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsPublic { get; set; } = true;
    public bool IsAutoAdd { get; set; }
    public GroupType Type { get; set; } = GroupType.Public;
    public Guid CreatorId { get; set; }

    public ICollection<GroupMember> Members { get; set; } = new List<GroupMember>();
    public ICollection<Post> Posts { get; set; } = new List<Post>();

    public void Update(string name, string description, bool isPublic, bool isAutoAdd, GroupType type)
    {
        Name = name;
        Description = description;
        IsPublic = isPublic;
        IsAutoAdd = isAutoAdd;
        Type = type;
    }
}
