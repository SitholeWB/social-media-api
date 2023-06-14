using SocialMediaApi.Domain.Events.GroupPostComments;
using SocialMediaApi.Domain.Events.GroupPosts;
using SocialMediaApi.Domain.Events.Groups;
using SocialMediaApi.Logic.EventHandlers;
using SocialMediaApi.Logic.EventHandlers.GroupPostComments;
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
            //Group Post Comments
            EventHandlerContainer.Subscribe<AddGroupPostCommentEvent, AddGroupPostCommentNotificationHandler>();
            EventHandlerContainer.Subscribe<DeleteGroupPostCommentEvent, DeleteGroupPostCommentNotificationHandler>();
            return services;
        }
    }
}