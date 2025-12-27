namespace SocialMedia.Domain;

public class MediaFile : BaseEntity
{
    public string FileName { get; set; } = string.Empty;
    public string Url { get; set; } = string.Empty;
}