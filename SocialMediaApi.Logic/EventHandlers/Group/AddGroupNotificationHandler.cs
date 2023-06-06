using SocialMediaApi.Domain.Events.Groups;
using SocialMediaApi.Interfaces;
using System.Diagnostics;

namespace SocialMediaApi.Logic.EventHandlers.Group
{
    public class AddGroupNotificationHandler : IEventHandler<AddGroupEvent>
    {
        public async Task RunAsync(AddGroupEvent obj)
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