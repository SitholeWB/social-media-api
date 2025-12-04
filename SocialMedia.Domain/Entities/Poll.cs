
namespace SocialMedia.Domain;

public class Poll : BaseEntity
{
    public string Question { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public DateTime? ExpiresAt { get; set; }
    public Guid CreatorId { get; set; }

    public ICollection<PollOption> Options { get; set; } = new List<PollOption>();

    public void Update(string question, bool isActive, DateTime? expiresAt)
    {
        Question = question;
        IsActive = isActive;
        ExpiresAt = expiresAt;
    }
}
