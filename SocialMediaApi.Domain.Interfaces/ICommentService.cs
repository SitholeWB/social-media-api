using Pagination.EntityFrameworkCore.Extensions;
using SocialMediaApi.Domain.Enums;
using SocialMediaApi.Domain.Models.Comments;
using SocialMediaApi.Domain.Models.Security;
using SocialMediaApi.Domain.ViewModels;

namespace SocialMediaApi.Domain.Interfaces
{
	public interface ICommentService
	{
		public Task<CommentViewModel> AddCommentAsync(AuthUser authUser, Guid postId, AddCommentModel model);

		public Task<CommentViewModel> UpdateCommentAsync(AuthUser authUser, Guid postId, Guid id, UpdateCommentModel model);

		public Task UpdateCommentExpireDateAsync(Guid postId, Guid id, EntityActionType entityActionType);

		public Task DeleteCommentAsync(AuthUser authUser, Guid postId, Guid id);

		public Task<Pagination<CommentViewModel>> GetCommentsAsync(AuthUser authUser, Guid postId, int page = 1, int limit = 20);
	}
}