namespace SocialMediaApi.Domain.Entities.Base
{
    public class BaseUser
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = "Unknown";
        public string ImageUrl { get; set; } = string.Empty;
    }
}