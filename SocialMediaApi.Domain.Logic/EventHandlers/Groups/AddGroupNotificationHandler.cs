using SocialMediaApi.Domain.Events.Groups;
using SocialMediaApi.Domain.Interfaces;

namespace SocialMediaApi.Domain.Logic.EventHandlers.Groups
{
    public class AddGroupNotificationHandler : IEventHandler<AddGroupEvent>
    {
        public async Task RunAsync(AddGroupEvent obj)
        {
            await Task.CompletedTask;
        }
    }
}