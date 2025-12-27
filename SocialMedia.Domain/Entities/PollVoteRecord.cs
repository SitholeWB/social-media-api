namespace SocialMedia.Domain;

public class PollVoteRecord : BaseEntity
{
    public Guid PollId { get; set; }
    public Guid UserId { get; set; }
}