using Pagination.EntityFrameworkCore.Extensions;
using SocialMediaApi.Domain.Models.Reactions;
using SocialMediaApi.Domain.ViewModels;

namespace SocialMediaApi.Interfaces
{
    public interface IReactionService
    {
        public Task<ReactionViewModel> AddReactionAsync(AddReactionModel model);

        public Task DeleteReactionAsync(Guid entityId, Guid id);

        public Task<Pagination<ReactionViewModel>> GetReactionsAsync(Guid entityId, int page = 1, int limit = 20);
    }
}