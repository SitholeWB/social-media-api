namespace SocialMediaApi.Domain.Models.UserPosts
{
    public class AddUserPostModel
    {
        public Guid EntityId { get; set; }
        public Guid UserId { get; set; }
        public DateTimeOffset CreatedDate { get; set; }
    }
}