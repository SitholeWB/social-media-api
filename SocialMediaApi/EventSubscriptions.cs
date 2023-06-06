using SocialMediaApi.Domain.Events.GroupPosts;
using SocialMediaApi.Domain.Events.Groups;
using SocialMediaApi.Logic.EventHandlers;
using SocialMediaApi.Logic.EventHandlers.GroupPosts;
using SocialMediaApi.Logic.EventHandlers.Groups;

namespace SocialMediaApi
{
    public static class EventSubscriptions
    {
        public static IServiceCollection AddEventSubscriptions(this IServiceCollection services)
        {
            //Groups
            EventHandlerContainer.Subscribe<AddGroupEvent, AddGroupNotificationHandler>();
            EventHandlerContainer.Subscribe<DeleteGroupEvent, DeleteGroupNotificationHandler>();
            EventHandlerContainer.Subscribe<UpdateGroupEvent, UpdateGroupNotificationHandler>();
            //Group Posts
            EventHandlerContainer.Subscribe<AddGroupPostEvent, AddGroupPostNotificationHandler>();
            EventHandlerContainer.Subscribe<DeleteGroupPostEvent, DeleteGroupPostNotificationHandler>();
            EventHandlerContainer.Subscribe<UpdateGroupPostEvent, UpdateGroupPostNotificationHandler>();
            return services;
        }
    }
}