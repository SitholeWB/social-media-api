using SocialMediaApi.Domain.Entities;

namespace SocialMediaApi.Domain.Events.Groups
{
    public class DeleteGroupEvent : EventBase
    {
        public Group Group { get; set; } = default!;
    }
}