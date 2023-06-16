using Pagination.EntityFrameworkCore.Extensions;
using SocialMediaApi.Domain.Models.UserPosts;
using SocialMediaApi.Domain.ViewModels;

namespace SocialMediaApi.Interfaces
{
    public interface IUserPostService
    {
        public Task<UserPostViewModel> AddUserPostAsync(AddUserPostModel model);

        public Task DeleteUserPostAsync(Guid entityId);

        public Task<Pagination<UserPostViewModel>> GetUserPostsAsync(Guid userId, int page);
    }
}