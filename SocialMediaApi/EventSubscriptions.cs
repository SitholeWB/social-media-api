using SocialMediaApi.Domain.Events.Groups;
using SocialMediaApi.Logic.EventHandlers;
using SocialMediaApi.Logic.EventHandlers.Groups;

namespace SocialMediaApi
{
    public static class EventSubscriptions
    {
        public static IServiceCollection AddEventSubscriptions(this IServiceCollection services)
        {
            EventHandlerContainer.Subscribe<AddGroupEvent, AddGroupNotificationHandler>();
            EventHandlerContainer.Subscribe<DeleteGroupEvent, DeleteGroupNotificationHandler>();
            EventHandlerContainer.Subscribe<UpdateGroupEvent, UpdateGroupNotificationHandler>();
            return services;
        }
    }
}