using SocialMediaApi.Domain.Events.Groups;
using SocialMediaApi.Domain.Events.PostComments;
using SocialMediaApi.Domain.Events.Posts;
using SocialMediaApi.Logic.EventHandlers;
using SocialMediaApi.Logic.EventHandlers.Groups;
using SocialMediaApi.Logic.EventHandlers.PostComments;
using SocialMediaApi.Logic.EventHandlers.Posts;

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
            EventHandlerContainer.Subscribe<AddPostEvent, AddPostNotificationHandler>();
            EventHandlerContainer.Subscribe<DeletePostEvent, DeletePostNotificationHandler>();
            EventHandlerContainer.Subscribe<UpdatePostEvent, UpdatePostNotificationHandler>();
            //Group Post Comments
            EventHandlerContainer.Subscribe<AddPostCommentEvent, AddPostCommentNotificationHandler>();
            EventHandlerContainer.Subscribe<DeletePostCommentEvent, DeletePostCommentNotificationHandler>();
            return services;
        }
    }
}