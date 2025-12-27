namespace SocialMedia.Domain;

public class Poll : BaseEntity
{
    public string Question { get; set; } = string.Empty;
    public bool IsActive { get; set; } = true;
    public DateTime? ExpiresAt { get; set; }
    public Guid CreatorId { get; set; }
    public bool IsAnonymous { get; set; } = false;
    public Guid GroupId { get; set; }
    public Group? Group { get; set; }

    public ICollection<PollOption> Options { get; set; } = new List<PollOption>();

    public void Update(string question, bool isActive, DateTime? expiresAt, bool isAnonymous, Guid groupId)
    {
        Question = question;
        IsActive = isActive;
        ExpiresAt = expiresAt;
        IsAnonymous = isAnonymous;
        GroupId = groupId;
    }
}