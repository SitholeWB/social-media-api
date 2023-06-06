using SocialMediaApi.Domain.Events.GroupPosts;
using SocialMediaApi.Interfaces;
using System.Diagnostics;

namespace SocialMediaApi.Logic.EventHandlers.GroupPosts
{
    public class AddGroupPostNotificationHandler : IEventHandler<AddGroupPostEvent>
    {
        public async Task RunAsync(AddGroupPostEvent obj)
        {
            Trace.WriteLine("!!!!!!!!!!!!!!!!!!!!!!!!!");
            await Task.Delay(TimeSpan.FromSeconds(20));
            Trace.WriteLine("???????????????????");
            /*return Task.Run(() =>
            {
                Console.WriteLine("!!!!!!!!!!!!!!!!!!!!!!!!!");
                Trace.WriteLine("???????????????????");
                Console.WriteLine("Confirm Email Sent.");
            });*/
        }
    }
}