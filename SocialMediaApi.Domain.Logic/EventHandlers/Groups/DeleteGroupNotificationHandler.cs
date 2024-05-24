using SocialMediaApi.Domain.Events.Groups;
using SubPub.Hangfire;

namespace SocialMediaApi.Domain.Logic.EventHandlers.Groups
{
    public class DeleteGroupNotificationHandler : IHangfireEventHandler<DeleteGroupEvent>
    {
        public async Task RunAsync(DeleteGroupEvent obj)
        {
            await Task.CompletedTask;
        }
    }
}