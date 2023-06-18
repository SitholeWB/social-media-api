using SocialMediaApi.Domain.Events.Posts;
using SocialMediaApi.Interfaces;
using SocialMediaApi.Interfaces.UnitOfWork;

namespace SocialMediaApi.Logic.EventHandlers.Posts
{
    public class DeletePostNotificationHandler : IEventHandler<DeletePostEvent>
    {
        private readonly IActivePostService _newPostService;
        private readonly IUserPostService _userPostService;

        public DeletePostNotificationHandler(IPostUnitOfWork postUnitOfWork)
        {
            _newPostService = postUnitOfWork.ActivePostService;
            _userPostService = postUnitOfWork.UserPostService;
        }

        public async Task RunAsync(DeletePostEvent obj)
        {
            if (obj?.Post != null)
            {
                await _userPostService.DeleteUserPostAsync(obj.Post!.Creator.Id, obj.Post!.Id);
                await _newPostService.DeleteActivePostAsync(obj.Post!.OwnerId, obj.Post.Id);
            }
        }
    }
}