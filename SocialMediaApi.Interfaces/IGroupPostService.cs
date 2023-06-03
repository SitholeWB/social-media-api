using Pagination.EntityFrameworkCore.Extensions;
using SocialMediaApi.Domain.Models.GroupPost;
using SocialMediaApi.Domain.ViewModels;

namespace SocialMediaApi.Interfaces
{
    public interface IGroupPostService
    {
        public Task<GroupPostViewModel> AddGroupPostAsync(Guid groupId, AddGroupPostModel model);

        public Task<GroupPostViewModel> UpdateGroupPostAsync(Guid groupId, Guid id, UpdateGroupPostModel model);

        public Task DeleteGroupPostAsync(Guid groupId, Guid id);

        public Task<GroupPostViewModel> GetGroupPostAsync(Guid groupId, Guid id);

        public Task<Pagination<GroupPostViewModel>> GetGroupPostsAsync(Guid groupId, int page = 1, int limit = 20);
    }
}