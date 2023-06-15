using SocialMediaApi.Domain.Models.Reactions;
using SocialMediaApi.Domain.ViewModels;

namespace SocialMediaApi.Interfaces
{
    public interface IUserReactionService
    {
        public Task AddReactionAsync(AddReactionModel model);

        public Task DeleteReactionAsync(Guid entityId);

        public Task<UserReactionViewModel?> GetReactionAsync(Guid entityId);
    }
}