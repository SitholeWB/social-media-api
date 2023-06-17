namespace SocialMediaApi.Domain.DTOs
{
    public class TokenDto
    {
        public string Token { get; set; }
        public DateTimeOffset ExpireOn { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
    }
}