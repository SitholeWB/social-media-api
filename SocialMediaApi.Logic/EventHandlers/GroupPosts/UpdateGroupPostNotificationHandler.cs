using SocialMediaApi.Domain.Events.GroupPosts;
using SocialMediaApi.Domain.Models.ActiveGroupPosts;
using SocialMediaApi.Interfaces;
using SocialMediaApi.Interfaces.UnitOfWork;

namespace SocialMediaApi.Logic.EventHandlers.GroupPosts
{
    public class UpdateGroupPostNotificationHandler : IEventHandler<UpdateGroupPostEvent>
    {
        private readonly IActiveGroupPostService _newGroupPostService;

        public UpdateGroupPostNotificationHandler(IPostUnitOfWork postUnitOfWork)
        {
            _newGroupPostService = postUnitOfWork.ActiveGroupPostService;
        }

        public async Task RunAsync(UpdateGroupPostEvent obj)
        {
            if (obj?.GroupPost != null)
            {
                await _newGroupPostService.UpdateActiveGroupPostAsync(obj.GroupPost!.GroupId, obj.GroupPost.Id, new UpdateActiveGroupPostModel
                {
                    GroupPost = obj.GroupPost
                });
            }
        }
    }
}