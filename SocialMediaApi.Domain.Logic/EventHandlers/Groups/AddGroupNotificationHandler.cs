using SocialMediaApi.Domain.Events.Groups;
using SubPub.Hangfire;

namespace SocialMediaApi.Domain.Logic.EventHandlers.Groups
{
    public class AddGroupNotificationHandler : IHangfireEventHandler<AddGroupEvent>
    {
        public async Task RunAsync(AddGroupEvent obj)
        {
            await Task.CompletedTask;
        }
    }
}