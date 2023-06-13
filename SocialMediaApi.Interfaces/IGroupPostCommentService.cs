using Pagination.EntityFrameworkCore.Extensions;
using SocialMediaApi.Domain.Enums;
using SocialMediaApi.Domain.Models.GroupPostComments;
using SocialMediaApi.Domain.ViewModels;

namespace SocialMediaApi.Interfaces
{
    public interface IGroupPostCommentService
    {
        public Task<GroupPostCommentViewModel> AddGroupPostCommentAsync(Guid groupPostId, AddGroupPostCommentModel model);

        public Task<GroupPostCommentViewModel> UpdateGroupPostCommentAsync(Guid groupPostId, Guid id, UpdateGroupPostCommentModel model);

        public Task UpdateGroupPostCommentExpireDateAsync(Guid groupPostId, Guid id, EntityActionType entityActionType);

        public Task DeleteGroupPostCommentAsync(Guid groupPostId, Guid id);

        public Task<Pagination<GroupPostCommentViewModel>> GetGroupPostCommentsAsync(Guid groupPostId, int page = 1, int limit = 20);
    }
}