using SocialMediaApi.Domain.Events.Posts;
using SocialMediaApi.Interfaces;
using SocialMediaApi.Interfaces.UnitOfWork;

namespace SocialMediaApi.Logic.EventHandlers.Posts
{
    public class DeletePostNotificationHandler : IEventHandler<DeletePostEvent>
    {
        private readonly IActivePostService _newPostService;

        public DeletePostNotificationHandler(IPostUnitOfWork postUnitOfWork)
        {
            _newPostService = postUnitOfWork.ActivePostService;
        }

        public async Task RunAsync(DeletePostEvent obj)
        {
            if (obj?.Post != null)
            {
                await _newPostService.DeleteActivePostAsync(obj.Post!.GroupId, obj.Post.Id);
            }
        }
    }
}