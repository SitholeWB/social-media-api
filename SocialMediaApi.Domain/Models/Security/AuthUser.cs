using SocialMediaApi.Domain.Entities.Base;

namespace SocialMediaApi.Domain.Models.Security
{
	public class AuthUser
	{
		public bool IsAuthenticated { get; set; }
		public BaseUser AuthenticatedUser { get; set; }
		public BaseUser AuthorizedUser { get; set; }
	}
}