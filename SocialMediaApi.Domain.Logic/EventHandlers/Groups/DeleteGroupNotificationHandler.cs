﻿using SocialMediaApi.Domain.Events.Groups;
using SocialMediaApi.Domain.Interfaces;

namespace SocialMediaApi.Domain.Logic.EventHandlers.Groups
{
    public class DeleteGroupNotificationHandler : IEventHandler<DeleteGroupEvent>
    {
        public async Task RunAsync(DeleteGroupEvent obj)
        {
            await Task.CompletedTask;
        }
    }
}