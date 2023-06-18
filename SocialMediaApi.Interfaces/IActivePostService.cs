using Pagination.EntityFrameworkCore.Extensions;
using SocialMediaApi.Domain.Enums;
using SocialMediaApi.Domain.Models.ActivePosts;
using SocialMediaApi.Domain.ViewModels;

namespace SocialMediaApi.Interfaces
{
    public interface IActivePostService
    {
        public Task AddActivePostAsync(Guid ownerId, AddActivePostModel model);

        public Task UpdateActivePostAsync(Guid ownerId, Guid id, UpdateActivePostModel model);

        public Task UpdateActivePostExpireDateAsync(Guid ownerId, Guid id, EntityActionType entityActionType);

        public Task UpdateActiveCommentCountAsync(Guid ownerId, Guid id, bool increment);

        public Task DeleteActivePostAsync(Guid ownerId, Guid id);

        public Task DeleteExpiredActivePostAsync();

        public Task<PostViewModel?> GetActivePostAsync(Guid ownerId, Guid id);

        public Task<Pagination<PostViewModel>> GetActivePostsAsync(Guid ownerId, int page = 1, int limit = 20);
    }
}