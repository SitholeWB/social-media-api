
namespace SocialMedia.Domain;

public class Block : BaseEntity
{
    public long Index { get; set; }
    public DateTime Timestamp { get; set; }
    public Guid VoteId { get; set; }
    public string PreviousHash { get; set; } = string.Empty;
    public string Hash { get; set; } = string.Empty;
    public long Nonce { get; set; }
}
