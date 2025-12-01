namespace SocialMedia.Domain;

public class CommentReadDto
{
    public Guid Id { get; set; }
    public string Content { get; set; } = string.Empty;
    public Guid AuthorId { get; set; }
    public string AuthorName { get; set; } = string.Empty;
    public string? AuthorProfilePicUrl { get; set; }
    public DateTime CreatedAt { get; set; }

    public int LikeCount { get; set; }
    public ICollection<ReactionReadDto> Reactions { get; set; } = new List<ReactionReadDto>();
}
