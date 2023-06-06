using SocialMediaApi.Domain.Events;
using SocialMediaApi.Interfaces;
using System.Diagnostics;

namespace SocialMediaApi.Logic.EventHandlers
{
    public class DeleteGroupNotificationHandler : IEventHandler<DeleteGroupEvent>
    {
        public void Run(DeleteGroupEvent obj)
        {
            Console.WriteLine("!!!!!!!!!!!!!!!!!!!!!!!!!");
            Trace.WriteLine("???????????????????");
            Console.WriteLine("Confirm Email Sent.");
        }

        public async Task RunAsync(DeleteGroupEvent obj)
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