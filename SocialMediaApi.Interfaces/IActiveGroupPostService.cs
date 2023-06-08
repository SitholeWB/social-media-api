using Pagination.EntityFrameworkCore.Extensions;
using SocialMediaApi.Domain.Models.ActiveGroupPosts;
using SocialMediaApi.Domain.ViewModels;

namespace SocialMediaApi.Interfaces
{
    public interface IActiveGroupPostService
    {
        public Task<GroupPostViewModel> AddActiveGroupPostAsync(Guid groupId, AddActiveGroupPostModel model);

        public Task<GroupPostViewModel> UpdateActiveGroupPostAsync(Guid groupId, Guid id, UpdateActiveGroupPostModel model);

        public Task DeleteActiveGroupPostAsync(Guid groupId, Guid id);

        public Task<GroupPostViewModel?> GetActiveGroupPostAsync(Guid groupId, Guid id);

        public Task<Pagination<GroupPostViewModel>> GetActiveGroupPostsAsync(Guid groupId, int page = 1, int limit = 20);
    }
}