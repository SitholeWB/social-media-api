namespace SocialMedia.Domain;

public class PostReadModel
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public Guid AuthorId { get; set; }
    public string AuthorName { get; set; } = string.Empty; // Denormalized
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? LastModifiedAt { get; set; }
    public ICollection<MediaDto> Media { get; set; } = new List<MediaDto>();
    public bool StatusFullScreen { get; set; } = false;
    public ICollection<TagDto> AdminTags { get; set; } = new List<TagDto>();
    public ICollection<TagDto> Tags { get; set; } = new List<TagDto>();

    // Stats Stats
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