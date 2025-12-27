using System.ComponentModel.DataAnnotations;

namespace SocialMedia.Files.API.Data;

public class UserFile
{
    public Guid Id { get; set; }
    
    [Required]
    public string UserId { get; set; } = string.Empty;
    
    public Guid FileId { get; set; }
    
    [Required]
    public string FileName { get; set; } = string.Empty;
    
    [Required]
    public string Hash { get; set; } = string.Empty;
    
    public string? DatabaseName { get; set; }
    
    public string? MimeType { get; set; }
    
    public DateTime CreatedOn { get; set; }
}
