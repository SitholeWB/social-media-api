namespace SocialMedia.Domain;

public class PostReadModel
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public Guid AuthorId { get; set; }
    public string AuthorName { get; set; } = string.Empty; // Denormalized
    public DateTime CreatedAt { get; set; }
    public string? FileUrl { get; set; }

    // Stats
    // Stats
    public PostStatsDto Stats { get; set; } = new();

    // Enriched Data
    public ICollection<ReactionReadDto> Reactions { get; set; } = new List<ReactionReadDto>();
    public ICollection<CommentReadDto> TopComments { get; set; } = new List<CommentReadDto>();

    // Group Info
    public Guid? GroupId { get; set; }
    public string? GroupName { get; set; }

    public void UpdateTrendingScore()
    {
        Stats.TrendingScore = Stats.LikeCount + (Stats.CommentCount * 2);
    }
}
