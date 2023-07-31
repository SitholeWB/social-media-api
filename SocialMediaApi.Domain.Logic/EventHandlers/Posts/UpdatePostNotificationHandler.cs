using SocialMediaApi.Domain.Events.Posts;
using SocialMediaApi.Domain.Models.ActivePosts;
using SocialMediaApi.Domain.Interfaces;
using SocialMediaApi.Domain.Interfaces.UnitOfWork;

namespace SocialMediaApi.Domain.Logic.EventHandlers.Posts
{
    public class UpdatePostNotificationHandler : IEventHandler<UpdatePostEvent>
    {
        private readonly IActivePostService _newPostService;

        public UpdatePostNotificationHandler(IPostUnitOfWork postUnitOfWork)
        {
            _newPostService = postUnitOfWork.ActivePostService;
        }

        public async Task RunAsync(UpdatePostEvent obj)
        {
            if (obj?.Post != null)
            {
                await _newPostService.UpdateActivePostAsync(obj.Post!.OwnerId, obj.Post.Id, new UpdateActivePostModel
                {
                    Post = obj.Post
                });
            }
        }
    }
}