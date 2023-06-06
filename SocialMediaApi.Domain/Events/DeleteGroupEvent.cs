using SocialMediaApi.Domain.Entities;

namespace SocialMediaApi.Domain.Events
{
    public class DeleteGroupEvent : EventBase
    {
        public Group Group { get; set; } = default!;
    }
}