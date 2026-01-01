namespace SocialMedia.Application;

public class PostDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public Guid GroupId { get; set; }
    public Guid AuthorId { get; set; }
    public string AuthorName { get; set; } = string.Empty;
    public ICollection<MediaDto> Media { get; set; } = [];
    public bool StatusFullScreen { get; set; } = false;
    public string? CreatedBy { get; set; } = string.Empty;
    public DateTimeOffset CreatedAt { get; set; }
    public int LikeCount { get; set; }
    public int CommentCount { get; set; }
    public string? UserReaction { get; set; }
    public ICollection<ReactionReadDto> Reactions { get; set; } = new List<ReactionReadDto>();
    public ICollection<TagDto> AdminTags { get; set; } = [];
    public ICollection<TagDto> Tags { get; set; } = [];
}