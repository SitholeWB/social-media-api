using SocialMediaApi.Domain.Events.Comments;
using SocialMediaApi.Domain.Events.Groups;
using SocialMediaApi.Domain.Events.Posts;
using SocialMediaApi.Logic.EventHandlers;
using SocialMediaApi.Logic.EventHandlers.Comments;
using SocialMediaApi.Logic.EventHandlers.Groups;
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
            //Posts
            EventHandlerContainer.Subscribe<AddPostEvent, AddPostNotificationHandler>();
            EventHandlerContainer.Subscribe<DeletePostEvent, DeletePostNotificationHandler>();
            EventHandlerContainer.Subscribe<UpdatePostEvent, UpdatePostNotificationHandler>();
            //Comments
            EventHandlerContainer.Subscribe<AddCommentEvent, AddCommentNotificationHandler>();
            EventHandlerContainer.Subscribe<DeleteCommentEvent, DeleteCommentNotificationHandler>();
            return services;
        }
    }
}