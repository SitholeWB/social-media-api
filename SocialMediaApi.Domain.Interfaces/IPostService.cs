using Pagination.EntityFrameworkCore.Extensions;
using SocialMediaApi.Domain.Enums;
using SocialMediaApi.Domain.Models.Posts;
using SocialMediaApi.Domain.Models.Security;
using SocialMediaApi.Domain.ViewModels;

namespace SocialMediaApi.Domain.Interfaces
{
	public interface IPostService
	{
		public Task<PostViewModel> AddPostAsync(AuthUser authUser, Guid ownerId, AddPostModel model);

		public Task<PostViewModel> UpdatePostAsync(AuthUser authUser, Guid ownerId, Guid id, UpdatePostModel model);

		public Task UpdatePostExpireDateAsync(Guid ownerId, Guid id, EntityActionType entityActionType);

		public Task DeletePostAsync(AuthUser authUser, Guid ownerId, Guid id);

		public Task<PostViewModel?> GetPostAsync(AuthUser authUser, Guid ownerId, Guid id);

		public Task<Pagination<PostViewModel>> GetPostsAsync(AuthUser authUser, Guid ownerId, int page = 1, int limit = 20);
	}
}