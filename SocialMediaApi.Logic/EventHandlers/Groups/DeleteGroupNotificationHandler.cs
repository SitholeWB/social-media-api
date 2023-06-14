using SocialMediaApi.Domain.Events.Groups;
using SocialMediaApi.Interfaces;

namespace SocialMediaApi.Logic.EventHandlers.Groups
{
    public class DeleteGroupNotificationHandler : IEventHandler<DeleteGroupEvent>
    {
        public async Task RunAsync(DeleteGroupEvent obj)
        {
            await Task.CompletedTask;
        }
    }
}