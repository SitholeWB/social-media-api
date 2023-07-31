using SocialMediaApi.Domain.Events.Comments;
using SocialMediaApi.Domain.Events.Groups;
using SocialMediaApi.Domain.Events.Posts;
using SocialMediaApi.Domain.Logic.EventHandlers;
using SocialMediaApi.Domain.Logic.EventHandlers.Comments;
using SocialMediaApi.Domain.Logic.EventHandlers.Groups;
using SocialMediaApi.Domain.Logic.EventHandlers.Posts;

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