using SocialMediaApi.Domain.Entities;

namespace SocialMediaApi.Domain.Events.Groups
{
    public class AddGroupEvent : EventBase
    {
        public Group? Group { get; set; }
    }
}