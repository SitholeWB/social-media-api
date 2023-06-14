using SocialMediaApi.Domain.Events.Groups;
using SocialMediaApi.Interfaces;

namespace SocialMediaApi.Logic.EventHandlers.Groups
{
    public class UpdateGroupNotificationHandler : IEventHandler<UpdateGroupEvent>
    {
        public async Task RunAsync(UpdateGroupEvent obj)
        {
            await Task.CompletedTask;
        }
    }
}