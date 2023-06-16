using SocialMediaApi.Domain.Entities.JsonEntities;
using SocialMediaApi.Domain.Models.Reactions;

namespace SocialMediaApi.Interfaces
{
    public interface IUserDetailsService
    {
        public Task AddCommentReactionAsync(Guid entityId, AddEntityReactionModel model);

        public Task AddPostReactionAsync(Guid entityId, AddEntityReactionModel model);

        public Task DeleteCommentReactionAsync(Guid entityId);

        public Task DeletePostReactionAsync(Guid entityId);

        public Task<IList<MiniReaction>> GetCommentReactionsAsync(Guid entityId);

        public Task<IList<MiniReaction>> GetPostReactionsAsync(Guid entityId);
    }
}