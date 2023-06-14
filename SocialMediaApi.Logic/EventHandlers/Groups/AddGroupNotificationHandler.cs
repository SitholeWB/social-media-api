using SocialMediaApi.Domain.Events.Groups;
using SocialMediaApi.Interfaces;

namespace SocialMediaApi.Logic.EventHandlers.Groups
{
    public class AddGroupNotificationHandler : IEventHandler<AddGroupEvent>
    {
        public async Task RunAsync(AddGroupEvent obj)
        {
            await Task.CompletedTask;
        }
    }
}