namespace SocialMedia.Domain;

public class CommentReadModel
{
    public Guid Id { get; set; }
    public Guid PostId { get; set; }
    public string Content { get; set; } = string.Empty;
    public Guid AuthorId { get; set; }
    public string AuthorName { get; set; } = string.Empty;
    public string? AuthorProfilePicUrl { get; set; }
    public ICollection<MediaDto> Media { get; set; } = new List<MediaDto>();
    public DateTimeOffset CreatedAt { get; set; }
    public DateTimeOffset? LastModifiedAt { get; set; }

    public CommentStatsDto Stats { get; set; } = new();

    public ICollection<ReactionReadDto> Reactions { get; set; } = new List<ReactionReadDto>();

    public ICollection<TagDto> AdminTags { get; set; } = new List<TagDto>();
    public ICollection<TagDto> Tags { get; set; } = new List<TagDto>();
}