using SocialMediaApi.Domain.Entities.JsonEntities;
using SocialMediaApi.Interfaces;

namespace SocialMediaApi.Logic.Helpers
{
    public struct UserDetailsReactionHelper
    {
        public static async Task<IList<MiniReaction>> GetPostReactionsAsync(IAuthService _authService, IUserDetailsService _userDetailsService)
        {
            var isAuthenticated = await _authService.IsAuthenticated();
            if (isAuthenticated)
            {
                return await _userDetailsService.GetPostReactionsAsync();
            }
            else
            {
                return new List<MiniReaction>();
            }
        }

        public static async Task<IList<MiniReaction>> GetCommentReactionsAsync(IAuthService _authService, IUserDetailsService _userDetailsService)
        {
            var isAuthenticated = await _authService.IsAuthenticated();
            if (isAuthenticated)
            {
                return await _userDetailsService.GetCommentReactionsAsync();
            }
            else
            {
                return new List<MiniReaction>();
            }
        }
    }
}