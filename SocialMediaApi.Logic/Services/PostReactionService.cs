using SocialMediaApi.Domain.Models.Reactions;
using SocialMediaApi.Domain.ViewModels;
using SocialMediaApi.Interfaces;

namespace SocialMediaApi.Logic.Services
{
    public class PostReactionService : IPostReactionService
    {
        public Task<UserReactionViewModel> AddReactionAsync(AddReactionModel model)
        {
            throw new NotImplementedException();
        }

        public Task DeleteReactionAsync(Guid entityId, Guid id)
        {
            throw new NotImplementedException();
        }
    }
}