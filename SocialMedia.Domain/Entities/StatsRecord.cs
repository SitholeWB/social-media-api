namespace SocialMedia.Domain;

public class StatsRecord : BaseEntity
{
    public StatsType StatsType { get; set; }
    public DateTimeOffset Date { get; set; }

    public int TotalPosts { get; set; }
    public int ActiveUsers { get; set; }

    // New Actions in Period
    public int NewPosts { get; set; }

    public int ResultingComments { get; set; }
    public int ResultingReactions { get; set; }

    public ICollection<ReactionStat> ReactionBreakdown { get; set; } = new List<ReactionStat>();
}