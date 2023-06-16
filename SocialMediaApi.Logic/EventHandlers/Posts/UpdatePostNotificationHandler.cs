using SocialMediaApi.Domain.Events.Posts;
using SocialMediaApi.Domain.Models.ActivePosts;
using SocialMediaApi.Interfaces;
using SocialMediaApi.Interfaces.UnitOfWork;

namespace SocialMediaApi.Logic.EventHandlers.Posts
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
                await _newPostService.UpdateActivePostAsync(obj.Post!.GroupId, obj.Post.Id, new UpdateActivePostModel
                {
                    Post = obj.Post
                });
            }
        }
    }
}