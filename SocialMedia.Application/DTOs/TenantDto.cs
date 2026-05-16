namespace SocialMedia.Application;

public class TenantDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? ThemeJson { get; set; }
    public DateTimeOffset CreatedAt { get; set; }
}
