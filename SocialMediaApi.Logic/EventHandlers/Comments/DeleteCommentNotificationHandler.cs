using SocialMediaApi.Domain.Events.Comments;
using SocialMediaApi.Interfaces;
using SocialMediaApi.Interfaces.UnitOfWork;

namespace SocialMediaApi.Logic.EventHandlers.Comments
{
    public class DeleteCommentNotificationHandler : IEventHandler<DeleteCommentEvent>
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