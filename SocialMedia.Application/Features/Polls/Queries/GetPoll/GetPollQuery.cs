namespace SocialMedia.Application;

public record GetPollQuery(Guid PollId) : IQuery<PollDto?>
{
    public Guid? UserId { get; set; }
}