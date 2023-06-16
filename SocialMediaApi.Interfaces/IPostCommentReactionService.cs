using SocialMediaApi.Domain.Models.Reactions;
using SocialMediaApi.Domain.ViewModels;

namespace SocialMediaApi.Interfaces
{
    public interface IPostCommentReactionService
    {
        public Task<EntityReactionViewModel> AddReactionAsync(AddEntityReactionModel model);

        public Task DeleteReactionAsync(Guid entityId, Guid id);
    }
}