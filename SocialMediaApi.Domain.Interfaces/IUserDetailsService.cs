using SocialMediaApi.Domain.Entities.JsonEntities;
using SocialMediaApi.Domain.Models.Reactions;
using SocialMediaApi.Domain.Models.Security;

namespace SocialMediaApi.Domain.Interfaces
{
	public interface IUserDetailsService
	{
		public Task AddCommentReactionAsync(AuthUser authUser, Guid entityId, AddEntityReactionModel model);

		public Task AddPostReactionAsync(AuthUser authUser, Guid entityId, AddEntityReactionModel model);

		public Task DeleteCommentReactionAsync(AuthUser authUser, Guid entityId);

		public Task DeletePostReactionAsync(AuthUser authUser, Guid entityId);

		public Task<IList<MiniReaction>> GetCommentReactionsAsync(AuthUser authUser);

		public Task<IList<MiniReaction>> GetPostReactionsAsync(AuthUser authUser);
	}
}