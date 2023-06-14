using SocialMediaApi.Domain.Models.Reactions;
using SocialMediaApi.Domain.ViewModels;

namespace SocialMediaApi.Interfaces
{
    public interface IPostReactionService
    {
        public Task<ReactionViewModel> AddReactionAsync(AddReactionModel model);

        public Task DeleteReactionAsync(Guid entityId, Guid id);
    }
}