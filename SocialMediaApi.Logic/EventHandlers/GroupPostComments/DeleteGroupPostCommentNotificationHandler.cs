using SocialMediaApi.Domain.Events.GroupPostComments;
using SocialMediaApi.Interfaces;
using SocialMediaApi.Interfaces.UnitOfWork;

namespace SocialMediaApi.Logic.EventHandlers.GroupPostComments
{
    public class DeleteGroupPostCommentNotificationHandler : IEventHandler<DeleteGroupPostCommentEvent>
    {
        private readonly IActiveGroupPostService _newGroupPostService;

        public DeleteGroupPostCommentNotificationHandler(IPostUnitOfWork postUnitOfWork)
        {
            _newGroupPostService = postUnitOfWork.ActiveGroupPostService;
        }

        public async Task RunAsync(DeleteGroupPostCommentEvent obj)
        {
            if (obj?.GroupPost != null && obj?.GroupPostComment != null)
            {
                await _newGroupPostService.UpdateActiveGroupPostCommentCountAsync(obj.GroupPostComment!.GroupPostId, obj.GroupPost!.GroupId, false);
            }
        }
    }
}