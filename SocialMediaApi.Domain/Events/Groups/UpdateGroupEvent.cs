using SocialMediaApi.Domain.Entities;

namespace SocialMediaApi.Domain.Events.Groups
{
    public class UpdateGroupEvent : EventBase
    {
        public Group? Group { get; set; }
    }
}