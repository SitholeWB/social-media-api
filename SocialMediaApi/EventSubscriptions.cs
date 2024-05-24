using SocialMediaApi.Domain.Events.Comments;
using SocialMediaApi.Domain.Events.Groups;
using SocialMediaApi.Domain.Events.Posts;
using SocialMediaApi.Domain.Logic.EventHandlers.Comments;
using SocialMediaApi.Domain.Logic.EventHandlers.Groups;
using SocialMediaApi.Domain.Logic.EventHandlers.Posts;
using SubPub.Hangfire;

namespace SocialMediaApi
{
    public static class EventSubscriptions
    {
        public static IServiceCollection AddEventSubscriptions(this IServiceCollection services)
        {
            //Groups
            services.AddHangfireSubPub<AddGroupEvent>().Subscribe<AddGroupNotificationHandler>();
            services.AddHangfireSubPub<DeleteGroupEvent>().Subscribe<DeleteGroupNotificationHandler>();
            services.AddHangfireSubPub<UpdateGroupEvent>().Subscribe<UpdateGroupNotificationHandler>();
            //Posts
            services.AddHangfireSubPub<AddPostEvent>().Subscribe<AddPostNotificationHandler>();
            services.AddHangfireSubPub<DeletePostEvent>().Subscribe<DeletePostNotificationHandler>();
            services.AddHangfireSubPub<UpdatePostEvent>().Subscribe<UpdatePostNotificationHandler>();
            //Comments
            services.AddHangfireSubPub<DeleteCommentEvent>().Subscribe<DeleteCommentNotificationHandler>();
            services.AddHangfireSubPub<AddCommentEvent>().Subscribe<AddCommentNotificationHandler>();
            return services;
        }
    }
}