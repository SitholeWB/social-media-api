namespace SocialMedia.Domain;

public class Comment : BaseEntity
{
    public string Content { get; set; } = string.Empty;
    public Guid PostId { get; set; }
    public Post Post { get; set; } = null!;
    public Guid AuthorId { get; set; }
    public bool IsDeleted { get; set; } = false;
    public string? CreatedBy { get; set; } = string.Empty;
    public ICollection<MediaDto> Media { get; set; } = new List<MediaDto>();
    public ICollection<Like> Likes { get; set; } = new List<Like>();

    public ICollection<TagDto> AdminTags { get; set; } = new List<TagDto>();
    public ICollection<TagDto> Tags { get; set; } = new List<TagDto>();
}