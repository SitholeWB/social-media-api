using SocialMediaApi.Logic.EventHandlers;
using SocialMediaApi.Logic.EventHandlers.GroupPosts;
using SocialMediaApi.Logic.EventHandlers.Groups;

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
            services.AddScoped<AddGroupPostNotificationHandler>();
            services.AddScoped<UpdateGroupPostNotificationHandler>();
            services.AddScoped<DeleteGroupPostNotificationHandler>();
            return services;
        }
    }
}