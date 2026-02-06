namespace SocialMedia.Domain;

public class UserActivity : BaseEntity
{
    public Guid UserId { get; set; }

    public DateTimeOffset? LastSeenAt { get; set; }

    public List<UserReactionActivity> Reactions { get; set; } = new();
    public List<UserVoteActivity> Votes { get; set; } = new();

    public void AddOrUpdateReaction(Guid entityId, string entityType, string emoji)
    {
        var existing = Reactions.FirstOrDefault(r => r.EntityId == entityId && r.EntityType == entityType);
        if (existing != null)
        {
            existing.Emoji = emoji;
        }
        else
        {
            Reactions.Add(new UserReactionActivity
            {
                EntityId = entityId,
                EntityType = entityType,
                Emoji = emoji
            });
        }
    }

    public void RemoveReaction(Guid entityId, string entityType)
    {
        Reactions.RemoveAll(r => r.EntityId == entityId && r.EntityType == entityType);
    }

    public void AddVote(Guid pollId, Guid optionId)
    {
        if (!Votes.Any(v => v.PollId == pollId))
        {
            Votes.Add(new UserVoteActivity
            {
                PollId = pollId,
                OptionId = optionId,
                VotedAt = DateTimeOffset.UtcNow
            });
        }
    }
}

public class UserReactionActivity
{
    public Guid EntityId { get; set; }
    public string EntityType { get; set; } = string.Empty;
    public string Emoji { get; set; } = string.Empty;
}

public class UserVoteActivity
{
    public Guid PollId { get; set; }
    public Guid OptionId { get; set; }
    public DateTimeOffset VotedAt { get; set; }
}