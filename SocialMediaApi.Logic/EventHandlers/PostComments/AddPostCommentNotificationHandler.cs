using SocialMediaApi.Domain.Events.PostComments;
using SocialMediaApi.Interfaces;
using SocialMediaApi.Interfaces.UnitOfWork;

namespace SocialMediaApi.Logic.EventHandlers.PostComments
{
    public class AddPostCommentNotificationHandler : IEventHandler<AddPostCommentEvent>
    {
        private readonly IActivePostService _newPostService;

        public AddPostCommentNotificationHandler(IPostUnitOfWork postUnitOfWork)
        {
            _newPostService = postUnitOfWork.ActivePostService;
        }

        public async Task RunAsync(AddPostCommentEvent obj)
        {
            if (obj?.Post != null && obj?.PostComment != null)
            {
                await _newPostService.UpdateActivePostCommentCountAsync(obj.Post!.GroupId, obj.PostComment!.PostId, true);
            }
        }
    }
}