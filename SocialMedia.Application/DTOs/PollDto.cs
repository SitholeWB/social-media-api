
namespace SocialMedia.Application;

public class PollDto
{
    public Guid Id { get; set; }
    public string Question { get; set; } = string.Empty;
    public bool IsActive { get; set; }
    public DateTime? ExpiresAt { get; set; }
    public Guid CreatorId { get; set; }
    public List<PollOptionDto> Options { get; set; } = new List<PollOptionDto>();
}

public class PollOptionDto
{
    public Guid Id { get; set; }
    public string Text { get; set; } = string.Empty;
    public int VoteCount { get; set; }
}
