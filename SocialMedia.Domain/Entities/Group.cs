namespace SocialMedia.Domain;

public class Group : BaseEntity
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public bool IsPublic { get; set; } = true;
    public bool IsAutoAdd { get; set; }

    public ICollection<GroupMember> Members { get; set; } = new List<GroupMember>();
    public ICollection<Post> Posts { get; set; } = new List<Post>();

    public void Update(string name, string description, bool isPublic, bool isAutoAdd)
    {
        Name = name;
        Description = description;
        IsPublic = isPublic;
        IsAutoAdd = isAutoAdd;
    }
}
