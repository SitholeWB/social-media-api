using Pagination.EntityFrameworkCore.Extensions;
using SocialMediaApi.Domain.Enums;
using SocialMediaApi.Domain.Models.ActiveGroupPosts;
using SocialMediaApi.Domain.ViewModels;

namespace SocialMediaApi.Interfaces
{
    public interface IActiveGroupPostService
    {
        public Task AddActiveGroupPostAsync(Guid groupId, AddActiveGroupPostModel model);

        public Task UpdateActiveGroupPostAsync(Guid groupId, Guid id, UpdateActiveGroupPostModel model);

        public Task UpdateActiveGroupPostRankAsync(Guid groupId, Guid id, EntityActionType entityActionType);

        public Task UpdateActiveGroupPostExpireDateAsync(Guid groupId, Guid id, EntityActionType entityActionType);

        public Task DeleteActiveGroupPostAsync(Guid groupId, Guid id);

        public Task<GroupPostViewModel?> GetActiveGroupPostAsync(Guid groupId, Guid id);

        public Task<Pagination<GroupPostViewModel>> GetActiveGroupPostsAsync(Guid groupId, int page = 1, int limit = 20);
    }
}