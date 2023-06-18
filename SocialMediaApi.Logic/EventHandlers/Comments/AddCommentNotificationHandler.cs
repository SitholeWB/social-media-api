using SocialMediaApi.Domain.Events.Comments;
using SocialMediaApi.Interfaces;
using SocialMediaApi.Interfaces.UnitOfWork;

namespace SocialMediaApi.Logic.EventHandlers.Comments
{
    public class AddCommentNotificationHandler : IEventHandler<AddCommentEvent>
    {
        private readonly IActivePostService _newPostService;

        public AddCommentNotificationHandler(IPostUnitOfWork postUnitOfWork)
        {
            _newPostService = postUnitOfWork.ActivePostService;
        }

        public async Task RunAsync(AddCommentEvent obj)
        {
            if (obj?.Post != null && obj?.Comment != null)
            {
                await _newPostService.UpdateActiveCommentCountAsync(obj.Post!.OwnerId, obj.Comment!.PostId, true);
            }
        }
    }
}