namespace SocialMedia.Domain;

public class Vote : BaseEntity
{
    public Guid PollOptionId { get; set; }
    public PollOption? PollOption { get; set; }
    public Guid? UserId { get; set; }
}