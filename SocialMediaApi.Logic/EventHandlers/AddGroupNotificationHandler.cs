using SocialMediaApi.Domain.Events;
using SocialMediaApi.Interfaces;
using System.Diagnostics;

namespace SocialMediaApi.Logic.EventHandlers
{
    public class AddGroupNotificationHandler : IEventHandler<AddGroupEvent>
    {
        public void Run(AddGroupEvent obj)
        {
            Console.WriteLine("!!!!!!!!!!!!!!!!!!!!!!!!!");
            Trace.WriteLine("???????????????????");
            Console.WriteLine("Confirm Email Sent.");
        }

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