using SocialMediaApi.Domain.Events.PostComments;
using SocialMediaApi.Interfaces;
using SocialMediaApi.Interfaces.UnitOfWork;

namespace SocialMediaApi.Logic.EventHandlers.PostComments
{
    public class DeletePostCommentNotificationHandler : IEventHandler<DeletePostCommentEvent>
    {
        private readonly IActivePostService _newPostService;

        public DeletePostCommentNotificationHandler(IPostUnitOfWork postUnitOfWork)
        {
            _newPostService = postUnitOfWork.ActivePostService;
        }

        public async Task RunAsync(DeletePostCommentEvent obj)
        {
            if (obj?.Post != null && obj?.PostComment != null)
            {
                await _newPostService.UpdateActivePostCommentCountAsync(obj.PostComment!.PostId, obj.Post!.GroupId, false);
            }
        }
    }
}