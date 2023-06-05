using SocialMediaApi.Domain.Events;
using SocialMediaApi.Logic.EventHandlers;

namespace SocialMediaApi
{
    public static class EventSubscriptions
    {
        public static IServiceCollection AddEventSubscriptions(this IServiceCollection services)
        {
            EventHandlerContainer.Subscribe<AddGroupEvent, AddGroupNotificationHandler>();
            return services;
        }
    }
}