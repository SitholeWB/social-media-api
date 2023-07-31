using SocialMediaApi.Domain.Events.Groups;
using SocialMediaApi.Domain.Interfaces;

namespace SocialMediaApi.Domain.Logic.EventHandlers.Groups
{
    public class UpdateGroupNotificationHandler : IEventHandler<UpdateGroupEvent>
    {
        public async Task RunAsync(UpdateGroupEvent obj)
        {
            await Task.CompletedTask;
        }
    }
}