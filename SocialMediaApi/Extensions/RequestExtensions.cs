using Microsoft.AspNetCore.Mvc;
using SocialMediaApi.Domain.Entities.Base;
using SocialMediaApi.Domain.Models.Security;

namespace SocialMediaApi.Extensions
{
	public static class RequestExtensions
	{
		public static AuthUser GetAuthUser(this ControllerBase httpRequest)
		{
			return new AuthUser
			{
				IsAuthenticated = httpRequest?.HttpContext?.User?.Identity?.IsAuthenticated ?? false,
				AuthenticatedUser = new BaseUser { },
				AuthorizedUser = new BaseUser { },
			};
		}
	}
}