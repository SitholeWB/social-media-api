namespace SocialMedia.Application;

public class GroupDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public GroupType Type { get; set; }
    public Guid CreatorId { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}