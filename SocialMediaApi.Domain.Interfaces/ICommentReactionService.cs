using SocialMediaApi.Domain.Models.Reactions;
using SocialMediaApi.Domain.Models.Security;
using SocialMediaApi.Domain.ViewModels;

namespace SocialMediaApi.Domain.Interfaces
{
	public interface ICommentReactionService
	{
		public Task<EntityReactionViewModel> AddReactionAsync(AuthUser authUser, Guid entityId, AddEntityReactionModel model);

		public Task<EntityReactionViewModel?> DeleteReactionAsync(AuthUser authUser, Guid entityId);
	}
}