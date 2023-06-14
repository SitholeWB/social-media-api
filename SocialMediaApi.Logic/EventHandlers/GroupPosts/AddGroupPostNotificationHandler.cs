using SocialMediaApi.Domain.Events.GroupPosts;
using SocialMediaApi.Domain.Models.ActiveGroupPosts;
using SocialMediaApi.Interfaces;
using SocialMediaApi.Interfaces.UnitOfWork;

namespace SocialMediaApi.Logic.EventHandlers.GroupPosts
{
    public class AddGroupPostNotificationHandler : IEventHandler<AddGroupPostEvent>
    {
        private readonly IActiveGroupPostService _newGroupPostService;

        public AddGroupPostNotificationHandler(IPostUnitOfWork postUnitOfWork)
        {
            _newGroupPostService = postUnitOfWork.ActiveGroupPostService;
        }

        public async Task RunAsync(AddGroupPostEvent obj)
        {
            if (obj?.GroupPost != null)
            {
                await _newGroupPostService.AddActiveGroupPostAsync(obj.GroupPost!.GroupId, new AddActiveGroupPostModel
                {
                    GroupPost = obj.GroupPost
                });
            }
        }
    }
}