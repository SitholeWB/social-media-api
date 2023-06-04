using MediatR;
using SocialMediaApi.Domain.Models.Events;

namespace SocialMediaApi.Logic.EventHandlers
{
    public sealed class AddGroupNotificationHandler : INotificationHandler<AddGroupEvent>
    {
        public Task Handle(AddGroupEvent group, CancellationToken cancellationToken)
        {
            // Could log to application insights or something...
            return default;
        }
    }
}