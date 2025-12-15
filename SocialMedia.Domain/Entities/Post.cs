namespace SocialMedia.Domain;

public class Post : BaseEntity
{
    public Guid GroupId { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Content { get; set; } = string.Empty;
    public Guid AuthorId { get; set; }
    public bool IsDeleted { get; set; } = false;
    public bool StatusFullScreen { get; set; } = false;
    public Guid? FileId { get; set; }
    public MediaFile? File { get; set; }
    public ICollection<TagDto> AdminTags { get; set; } = new List<TagDto>();
    public ICollection<TagDto> Tags { get; set; } = new List<TagDto>();
    public ICollection<Comment> Comments { get; set; } = [];
    public ICollection<Like> Likes { get; set; } = [];
    public virtual Group? Group { get; set; }
}