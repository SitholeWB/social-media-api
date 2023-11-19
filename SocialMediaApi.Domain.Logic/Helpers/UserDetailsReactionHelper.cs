using SocialMediaApi.Domain.Entities.JsonEntities;
using SocialMediaApi.Domain.Interfaces;
using SocialMediaApi.Domain.Models.Security;

namespace SocialMediaApi.Domain.Logic.Helpers
{
	public struct UserDetailsReactionHelper
	{
		public static async Task<IList<MiniReaction>> GetPostReactionsAsync(AuthUser authUser, IUserDetailsService _userDetailsService)
		{
			if (authUser.IsAuthenticated)
			{
				return await _userDetailsService.GetPostReactionsAsync(authUser);
			}
			else
			{
				return new List<MiniReaction>();
			}
		}

		public static async Task<IList<MiniReaction>> GetCommentReactionsAsync(AuthUser authUser, IUserDetailsService _userDetailsService)
		{
			if (authUser.IsAuthenticated)
			{
				return await _userDetailsService.GetCommentReactionsAsync(authUser);
			}
			else
			{
				return new List<MiniReaction>();
			}
		}
	}
}