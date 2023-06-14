using SocialMediaApi.Domain.Events.GroupPostComments;
using SocialMediaApi.Interfaces;
using SocialMediaApi.Interfaces.UnitOfWork;

namespace SocialMediaApi.Logic.EventHandlers.GroupPostComments
{
    public class AddGroupPostCommentNotificationHandler : IEventHandler<AddGroupPostCommentEvent>
    {
        private readonly IActiveGroupPostService _newGroupPostService;

        public AddGroupPostCommentNotificationHandler(IPostUnitOfWork postUnitOfWork)
        {
            _newGroupPostService = postUnitOfWork.ActiveGroupPostService;
        }

        public async Task RunAsync(AddGroupPostCommentEvent obj)
        {
            if (obj?.GroupPost != null && obj?.GroupPostComment != null)
            {
                await _newGroupPostService.UpdateActiveGroupPostCommentCountAsync(obj.GroupPostComment!.GroupPostId, obj.GroupPost!.GroupId, true);
            }
        }
    }
}