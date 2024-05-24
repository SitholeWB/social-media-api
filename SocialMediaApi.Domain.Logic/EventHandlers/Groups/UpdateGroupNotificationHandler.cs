using SocialMediaApi.Domain.Events.Groups;
using SubPub.Hangfire;

namespace SocialMediaApi.Domain.Logic.EventHandlers.Groups
{
    public class UpdateGroupNotificationHandler : IHangfireEventHandler<UpdateGroupEvent>
    {
        public async Task RunAsync(UpdateGroupEvent obj)
        {
            await Task.CompletedTask;
        }
    }
}