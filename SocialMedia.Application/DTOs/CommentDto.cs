namespace SocialMedia.Application;

public class CommentDto
{
    public Guid Id { get; set; }
    public string Content { get; set; } = string.Empty;
    public Guid PostId { get; set; }
    public Guid AuthorId { get; set; }
    public ICollection<MediaDto> Media { get; set; } = [];
    public DateTimeOffset CreatedAt { get; set; }
    public string? UserReaction { get; set; }
    public string? CreatedBy { get; set; } = string.Empty;
    public ICollection<ReactionReadDto> Reactions { get; set; } = new List<ReactionReadDto>();
    public ICollection<TagDto> AdminTags { get; set; } = [];
    public ICollection<TagDto> Tags { get; set; } = [];
}