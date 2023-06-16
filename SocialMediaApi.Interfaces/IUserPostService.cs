using Pagination.EntityFrameworkCore.Extensions;
using SocialMediaApi.Domain.Entities.JsonEntities;
using SocialMediaApi.Domain.Models.UserPosts;

namespace SocialMediaApi.Interfaces
{
    public interface IUserPostService
    {
        public Task AddUserPostAsync(AddUserPostModel model);

        public Task DeleteUserPostAsync(Guid userId, Guid entityId);

        public Task<Pagination<MiniEntity>> GetUserPostsAsync(Guid userId, int page, int limit);
    }
}