using MediatR;

namespace SocialMediaApi.Domain.Models.Events
{
    public class AddGroupEvent : INotification
    {
        public Entities.Group Group { get; set; } = default!;
    }
}