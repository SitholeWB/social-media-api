using Pagination.EntityFrameworkCore.Extensions;
using SocialMediaApi.Domain.Enums;
using SocialMediaApi.Domain.Models.PostComments;
using SocialMediaApi.Domain.ViewModels;

namespace SocialMediaApi.Interfaces
{
    public interface IPostCommentService
    {
        public Task<PostCommentViewModel> AddPostCommentAsync(Guid postId, AddPostCommentModel model);

        public Task<PostCommentViewModel> UpdatePostCommentAsync(Guid postId, Guid id, UpdatePostCommentModel model);

        public Task UpdatePostCommentExpireDateAsync(Guid postId, Guid id, EntityActionType entityActionType);

        public Task DeletePostCommentAsync(Guid postId, Guid id);

        public Task<Pagination<PostCommentViewModel>> GetPostCommentsAsync(Guid postId, int page = 1, int limit = 20);
    }
}