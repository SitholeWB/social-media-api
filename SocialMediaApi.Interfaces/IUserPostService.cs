using Pagination.EntityFrameworkCore.Extensions;
using SocialMediaApi.Domain.Models.UserPosts;
using SocialMediaApi.Domain.ViewModels;

namespace SocialMediaApi.Interfaces
{
    public interface IUserPostService
    {
        public Task AddUserPostAsync(AddUserPostModel model);

        public Task DeleteUserPostAsync(Guid userId, Guid entityId);

        public Task<Pagination<PostViewModel>> GetUserPostsAsync(Guid userId, int page);
    }
}