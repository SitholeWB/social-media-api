using SocialMediaApi.Domain.Entities;

namespace SocialMediaApi.Domain.Events
{
    public class UpdateGroupEvent : EventBase
    {
        public Group Group { get; set; } = default!;
    }
}