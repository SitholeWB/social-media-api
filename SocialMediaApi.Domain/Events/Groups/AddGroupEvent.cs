using SocialMediaApi.Domain.Entities;

namespace SocialMediaApi.Domain.Events.Groups
{
    public class AddGroupEvent
    {
        public Group? Group { get; set; }
    }
}