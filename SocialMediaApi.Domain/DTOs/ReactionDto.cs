namespace SocialMediaApi.Domain.DTOs
{
    public class ReactionDto
    {
        public bool Reacted { get; set; } = false;

        public string Unicode { get; set; } = string.Empty;
    }
}