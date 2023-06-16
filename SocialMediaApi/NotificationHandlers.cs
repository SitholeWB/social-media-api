using SocialMediaApi.Logic.EventHandlers;
using SocialMediaApi.Logic.EventHandlers.Groups;
using SocialMediaApi.Logic.EventHandlers.PostComments;
using SocialMediaApi.Logic.EventHandlers.Posts;

namespace SocialMediaApi
{
    public static class NotificationHandlers
    {
        public static IServiceCollection AddNotificationHandlers(this IServiceCollection services)
        {
            services.AddScoped<EventHandlerContainer>();
            //Groups
            services.AddScoped<AddGroupNotificationHandler>();
            services.AddScoped<UpdateGroupNotificationHandler>();
            services.AddScoped<DeleteGroupNotificationHandler>();
            //Group Posts
            services.AddScoped<AddPostNotificationHandler>();
            services.AddScoped<UpdatePostNotificationHandler>();
            services.AddScoped<DeletePostNotificationHandler>();
            //Group Post Comments
            services.AddScoped<AddPostCommentNotificationHandler>();
            services.AddScoped<DeletePostCommentNotificationHandler>();
            return services;
        }
    }
}