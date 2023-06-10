using SocialMediaApi.Domain.Events.GroupPosts;
using SocialMediaApi.Interfaces;
using SocialMediaApi.Interfaces.UnitOfWork;

namespace SocialMediaApi.Logic.EventHandlers.GroupPosts
{
    public class DeleteGroupPostNotificationHandler : IEventHandler<DeleteGroupPostEvent>
    {
        private readonly IActiveGroupPostService _newGroupPostService;

        public DeleteGroupPostNotificationHandler(IPostUnitOfWork postUnitOfWork)
        {
            _newGroupPostService = postUnitOfWork.ActiveGroupPostService;
        }

        public async Task RunAsync(DeleteGroupPostEvent obj)
        {
            await _newGroupPostService.DeleteActiveGroupPostAsync(obj.GroupPost.GroupId, obj.GroupPost.Id);
        }
    }
}