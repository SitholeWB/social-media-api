
namespace SocialMedia.Domain;

public class Post : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public Guid AuthorId { get; set; }

    public Guid? FileId { get; set; }
    public MediaFile? File { get; set; }

    public ICollection<Comment> Comments { get; set; } = new List<Comment>();
    public ICollection<Like> Likes { get; set; } = new List<Like>();
    public ICollection<Group> Groups { get; set; } = new List<Group>();
}
