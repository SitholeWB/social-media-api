namespace SocialMedia.Domain;

public class UserBlock : BaseEntity
{
    public Guid BlockerId { get; set; }
    public User Blocker { get; set; } = null!;

    public Guid BlockedUserId { get; set; }
    public User BlockedUser { get; set; } = null!;
}