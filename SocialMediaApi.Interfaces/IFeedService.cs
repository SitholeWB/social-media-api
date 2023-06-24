using Pagination.EntityFrameworkCore.Extensions;
using SocialMediaApi.Domain.ViewModels;

namespace SocialMediaApi.Interfaces
{
    public interface IFeedService
    {
        public Task<Pagination<PostViewModel>> GetPostsAsync(int page, int limit, bool skipActivePosts);

        public Task<Pagination<PostViewModel>> GetTrendingPostsAsync(int page, int limit);

        public Task<Pagination<PostViewModel>> GetAnonymousPostsAsync(int page, int limit, bool skipActivePosts);
    }
}