namespace SocialMedia.Domain;

public class Post : BaseEntity
{
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public Guid AuthorId { get; set; }

    public Guid? FileId { get; set; }
    public MediaFile? File { get; set; }
    public List<string> AdminTags { get; set; } = new List<string>();
    public List<string> Tags { get; set; } = new List<string>();
    public ICollection<Comment> Comments { get; set; } = [];
    public ICollection<Like> Likes { get; set; } = [];
    public ICollection<Group> Groups { get; set; } = [];
}