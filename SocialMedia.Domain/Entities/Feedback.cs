using System;

namespace SocialMedia.Domain;

public class Feedback : BaseEntity
{
    public string Content { get; set; } = string.Empty;
    public Guid UserId { get; set; }
    public virtual User User { get; set; } = default!;
}
