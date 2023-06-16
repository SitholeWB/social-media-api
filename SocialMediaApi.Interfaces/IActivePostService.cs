using Pagination.EntityFrameworkCore.Extensions;
using SocialMediaApi.Domain.Enums;
using SocialMediaApi.Domain.Models.ActivePosts;
using SocialMediaApi.Domain.ViewModels;

namespace SocialMediaApi.Interfaces
{
    public interface IActivePostService
    {
        public Task AddActivePostAsync(Guid groupId, AddActivePostModel model);

        public Task UpdateActivePostAsync(Guid groupId, Guid id, UpdateActivePostModel model);

        public Task UpdateActivePostExpireDateAsync(Guid groupId, Guid id, EntityActionType entityActionType);

        public Task UpdateActiveCommentCountAsync(Guid groupId, Guid id, bool increment);

        public Task DeleteActivePostAsync(Guid groupId, Guid id);

        public Task DeleteExpiredActivePostAsync();

        public Task<PostViewModel?> GetActivePostAsync(Guid groupId, Guid id);

        public Task<Pagination<PostViewModel>> GetActivePostsAsync(Guid groupId, int page = 1, int limit = 20);
    }
}