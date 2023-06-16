using SocialMediaApi.Domain.Events.Posts;
using SocialMediaApi.Domain.Models.ActivePosts;
using SocialMediaApi.Interfaces;
using SocialMediaApi.Interfaces.UnitOfWork;

namespace SocialMediaApi.Logic.EventHandlers.Posts
{
    public class AddPostNotificationHandler : IEventHandler<AddPostEvent>
    {
        private readonly IActivePostService _newPostService;

        public AddPostNotificationHandler(IPostUnitOfWork postUnitOfWork)
        {
            _newPostService = postUnitOfWork.ActivePostService;
        }

        public async Task RunAsync(AddPostEvent obj)
        {
            if (obj?.Post != null)
            {
                await _newPostService.AddActivePostAsync(obj.Post!.GroupId, new AddActivePostModel
                {
                    Post = obj.Post
                });
            }
        }
    }
}