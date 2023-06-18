using Pagination.EntityFrameworkCore.Extensions;
using SocialMediaApi.Domain.Enums;
using SocialMediaApi.Domain.Models.Posts;
using SocialMediaApi.Domain.ViewModels;

namespace SocialMediaApi.Interfaces
{
    public interface IPostService
    {
        public Task<PostViewModel> AddPostAsync(Guid ownerId, AddPostModel model);

        public Task<PostViewModel> UpdatePostAsync(Guid ownerId, Guid id, UpdatePostModel model);

        public Task UpdatePostExpireDateAsync(Guid ownerId, Guid id, EntityActionType entityActionType);

        public Task DeletePostAsync(Guid ownerId, Guid id);

        public Task<PostViewModel?> GetPostAsync(Guid ownerId, Guid id);

        public Task<Pagination<PostViewModel>> GetPostsAsync(Guid ownerId, int page = 1, int limit = 20);
    }
}