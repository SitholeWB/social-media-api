using Pagination.EntityFrameworkCore.Extensions;
using SocialMediaApi.Domain.Enums;
using SocialMediaApi.Domain.Models.Posts;
using SocialMediaApi.Domain.ViewModels;

namespace SocialMediaApi.Interfaces
{
    public interface IPostService
    {
        public Task<PostViewModel> AddPostAsync(Guid groupId, AddPostModel model);

        public Task<PostViewModel> UpdatePostAsync(Guid groupId, Guid id, UpdatePostModel model);

        public Task UpdatePostExpireDateAsync(Guid groupId, Guid id, EntityActionType entityActionType);

        public Task DeletePostAsync(Guid groupId, Guid id);

        public Task<PostViewModel?> GetPostAsync(Guid groupId, Guid id);

        public Task<Pagination<PostViewModel>> GetPostsAsync(Guid groupId, int page = 1, int limit = 20);
    }
}