namespace SocialMedia.Domain.ReadModels;

public class CommentReadModel
{
    public Guid Id { get; set; }
    public Guid PostId { get; set; }
    public string Content { get; set; } = string.Empty;
    public Guid AuthorId { get; set; }
    public string AuthorName { get; set; } = string.Empty;
    public string? AuthorProfilePicUrl { get; set; }
    public DateTime CreatedAt { get; set; }
    
    public CommentStats Stats { get; set; } = new();
    public ICollection<ReactionReadDto> Reactions { get; set; } = new List<ReactionReadDto>();
}
