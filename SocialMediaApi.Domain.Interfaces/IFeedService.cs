using Pagination.EntityFrameworkCore.Extensions;
using SocialMediaApi.Domain.Models.Security;
using SocialMediaApi.Domain.ViewModels;

namespace SocialMediaApi.Domain.Interfaces
{
	public interface IFeedService
	{
		public Task<Pagination<PostViewModel>> GetPostsAsync(AuthUser authUser, int page, int limit, bool skipActivePosts);

		public Task<Pagination<PostViewModel>> GetTrendingPostsAsync(AuthUser authUser, int page, int limit);

		public Task<Pagination<PostViewModel>> GetAnonymousPostsAsync(AuthUser authUser, int page, int limit, bool skipActivePosts);
	}
}