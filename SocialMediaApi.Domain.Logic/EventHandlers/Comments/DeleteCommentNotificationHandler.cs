using SocialMediaApi.Domain.Events.Comments;
using SocialMediaApi.Domain.Interfaces;
using SocialMediaApi.Domain.Interfaces.UnitOfWork;
using SubPub.Hangfire;

namespace SocialMediaApi.Domain.Logic.EventHandlers.Comments
{
    public class DeleteCommentNotificationHandler : IHangfireEventHandler<DeleteCommentEvent>
    {
        private readonly IActivePostService _newPostService;

        public DeleteCommentNotificationHandler(IPostUnitOfWork postUnitOfWork)
        {
            _newPostService = postUnitOfWork.ActivePostService;
        }

        public async Task RunAsync(DeleteCommentEvent obj)
        {
            if (obj?.Post != null && obj?.Comment != null)
            {
                await _newPostService.UpdateActiveCommentCountAsync(obj.Comment!.PostId, obj.Post!.OwnerId, false);
            }
        }
    }
}