namespace SocialMedia.Domain;

public class ReactionReadDto
{
    public Guid UserId { get; set; }
    public string UserName { get; set; } = string.Empty;
    public string? UserProfilePicUrl { get; set; }
    public string Emoji { get; set; } = "👍";
}
