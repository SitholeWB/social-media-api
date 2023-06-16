using SocialMediaApi.Domain.Models.Reactions;
using SocialMediaApi.Domain.ViewModels;

namespace SocialMediaApi.Interfaces
{
    public interface ICommentReactionService
    {
        public Task<EntityReactionViewModel> AddReactionAsync(AddEntityReactionModel model);

        public Task<EntityReactionViewModel?> DeleteReactionAsync(Guid entityId);
    }
}