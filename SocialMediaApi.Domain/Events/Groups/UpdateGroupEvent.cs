using SocialMediaApi.Domain.Entities;

namespace SocialMediaApi.Domain.Events.Groups
{
    public class UpdateGroupEvent
    {
        public Group? Group { get; set; }
    }
}