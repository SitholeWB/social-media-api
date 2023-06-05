﻿using SocialMediaApi.Logic.EventHandlers;

namespace SocialMediaApi
{
    public static class NotificationHandlers
    {
        public static IServiceCollection AddNotificationHandlers(this IServiceCollection services)
        {
            services.AddScoped<EventHandlerContainer>();
            services.AddScoped<AddGroupNotificationHandler>();
            return services;
        }
    }
}