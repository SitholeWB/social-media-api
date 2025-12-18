
namespace SocialMedia.Domain;

public class Poll : BaseEntity
{
    public string Question { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public DateTime? ExpiresAt { get; set; }
    public Guid CreatorId { get; set; }
    public bool IsAnonymous { get; set; } = false;

    public ICollection<PollOption> Options { get; set; } = new List<PollOption>();

    public void Update(string question, bool isActive, DateTime? expiresAt, bool isAnonymous)
    {
        Question = question;
        IsActive = isActive;
        ExpiresAt = expiresAt;
        IsAnonymous = isAnonymous;
    }
}
