using SocialMediaApi.Domain.Events.GroupPosts;
using SocialMediaApi.Domain.Models.NewGroupPosts;
using SocialMediaApi.Interfaces;

namespace SocialMediaApi.Logic.EventHandlers.GroupPosts
{
    public class AddGroupPostNotificationHandler : IEventHandler<AddGroupPostEvent>
    {
        private readonly INewGroupPostService _newGroupPostService;

        public AddGroupPostNotificationHandler(INewGroupPostService newGroupPostService)
        {
            _newGroupPostService = newGroupPostService;
        }

        public async Task RunAsync(AddGroupPostEvent obj)
        {
            await _newGroupPostService.AddNewGroupPostAsync(obj.GroupPost.GroupId, new AddNewGroupPostModel
            {
                GroupPost = obj.GroupPost
            });
        }
    }
}