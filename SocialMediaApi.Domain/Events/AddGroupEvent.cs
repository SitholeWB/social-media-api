using SocialMediaApi.Domain.Entities;

namespace SocialMediaApi.Domain.Events
{
    public class AddGroupEvent : EventBase
    {
        public Group Group { get; set; } = default!;
    }
}