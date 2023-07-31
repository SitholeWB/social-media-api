using SocialMediaApi.Domain.Logic.EventHandlers;
using SocialMediaApi.Domain.Logic.EventHandlers.Comments;
using SocialMediaApi.Domain.Logic.EventHandlers.Groups;
using SocialMediaApi.Domain.Logic.EventHandlers.Posts;

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
            //Posts
            services.AddScoped<AddPostNotificationHandler>();
            services.AddScoped<UpdatePostNotificationHandler>();
            services.AddScoped<DeletePostNotificationHandler>();
            //Comments
            services.AddScoped<AddCommentNotificationHandler>();
            services.AddScoped<DeleteCommentNotificationHandler>();
            return services;
        }
    }
}