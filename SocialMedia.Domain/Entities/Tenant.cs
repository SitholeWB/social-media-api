namespace SocialMedia.Domain;

public class Tenant : BaseEntity
{
    public static readonly Guid SuperTenantId = Guid.Parse("D0000000-0000-0000-0000-000000000001");

    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string? ThemeJson { get; set; }
}