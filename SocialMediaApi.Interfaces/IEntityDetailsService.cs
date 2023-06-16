using SocialMediaApi.Domain.Models.Reactions;
using SocialMediaApi.Domain.ViewModels;

namespace SocialMediaApi.Interfaces
{
    public interface IEntityDetailsService
    {
        public Task<EntityReactionViewModel> AddReactionAsync(Guid entityId, AddEntityReactionModel model);

        public Task<EntityReactionViewModel?> DeleteReactionAsync(Guid entityId);

        public Task<EntityReactionViewModel?> GetReactionAsync(Guid entityId);
    }
}