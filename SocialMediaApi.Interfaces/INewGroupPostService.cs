using Pagination.EntityFrameworkCore.Extensions;
using SocialMediaApi.Domain.Enums;
using SocialMediaApi.Domain.Models.NewGroupPosts;
using SocialMediaApi.Domain.ViewModels;

namespace SocialMediaApi.Interfaces
{
    public interface INewGroupPostService
    {
        public Task AddNewGroupPostAsync(Guid groupId, AddNewGroupPostModel model);

        public Task UpdateNewGroupPostAsync(Guid groupId, Guid id, UpdateNewGroupPostModel model);

        public Task UpdateNewGroupPostRankAsync(Guid groupId, Guid id, EntityActionType entityActionType);

        public Task DeleteNewGroupPostAsync(Guid groupId, Guid id);

        public Task<GroupPostViewModel?> GetNewGroupPostAsync(Guid groupId, Guid id);

        public Task<Pagination<GroupPostViewModel>> GetNewGroupPostsAsync(Guid groupId, int page = 1, int limit = 20);
    }
}