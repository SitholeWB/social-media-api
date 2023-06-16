using Pagination.EntityFrameworkCore.Extensions;
using SocialMediaApi.Domain.Enums;
using SocialMediaApi.Domain.Models.Comments;
using SocialMediaApi.Domain.ViewModels;

namespace SocialMediaApi.Interfaces
{
    public interface ICommentService
    {
        public Task<CommentViewModel> AddCommentAsync(Guid postId, AddCommentModel model);

        public Task<CommentViewModel> UpdateCommentAsync(Guid postId, Guid id, UpdateCommentModel model);

        public Task UpdateCommentExpireDateAsync(Guid postId, Guid id, EntityActionType entityActionType);

        public Task DeleteCommentAsync(Guid postId, Guid id);

        public Task<Pagination<CommentViewModel>> GetCommentsAsync(Guid postId, int page = 1, int limit = 20);
    }
}