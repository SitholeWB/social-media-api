
namespace SocialMedia.Domain;

public class PollOption : BaseEntity
{
    public string Text { get; set; } = string.Empty;
    public Guid PollId { get; set; }
    public Poll? Poll { get; set; }

    public ICollection<Vote> Votes { get; set; } = new List<Vote>();
}
