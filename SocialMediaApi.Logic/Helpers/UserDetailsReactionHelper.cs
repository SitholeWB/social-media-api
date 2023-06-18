using SocialMediaApi.Domain.Entities.JsonEntities;
using SocialMediaApi.Interfaces;

namespace SocialMediaApi.Logic.Helpers
{
    public static class UserDetailsReactionHelper
    {
        public static async Task<IList<MiniReaction>> GetPostReactionsAsync(IAuthService _authService, IUserDetailsService _userDetailsService)
        {
            var isAuthenticated = await _authService.IsAuthenticated();
            var reactions = default(IList<MiniReaction>);
            if (isAuthenticated)
            {
                reactions = await _userDetailsService.GetPostReactionsAsync();
            }
            else
            {
                reactions = new List<MiniReaction>();
            }

            return reactions ?? new List<MiniReaction>();
        }

        public static async Task<IList<MiniReaction>> GetCommentReactionsAsync(IAuthService _authService, IUserDetailsService _userDetailsService)
        {
            var isAuthenticated = await _authService.IsAuthenticated();
            var reactions = default(IList<MiniReaction>);
            if (isAuthenticated)
            {
                reactions = await _userDetailsService.GetCommentReactionsAsync();
            }
            else
            {
                reactions = new List<MiniReaction>();
            }

            return reactions ?? new List<MiniReaction>();
        }
    }
}