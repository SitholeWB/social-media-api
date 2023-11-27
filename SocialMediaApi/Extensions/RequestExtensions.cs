using Microsoft.AspNetCore.Mvc;
using SocialMediaApi.Domain.Entities.Base;
using SocialMediaApi.Domain.Models.Security;
using System.Security.Claims;

namespace SocialMediaApi.Extensions
{
	public static class RequestExtensions
	{
		public static AuthUser GetAuthUser(this ControllerBase httpRequest)
		{
			var userIdString = httpRequest?.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value;
			var name = httpRequest?.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value ?? "Unknown";
			var surname = httpRequest?.HttpContext?.User?.FindFirst(ClaimTypes.Surname)?.Value ?? "";
			var installationId = httpRequest?.HttpContext?.Request?.Headers["x-installation-id-header"].ToString() ?? "";
			return new AuthUser
			{
				IsAuthenticated = httpRequest?.HttpContext?.User?.Identity?.IsAuthenticated ?? false,
				AuthorizedUser = new BaseUser
				{
					Id = Guid.TryParse(userIdString, out var userId) ? userId : Guid.Empty,
					Name = $"{name} {surname}",
					InstallationId = installationId
				},
			};
		}
	}
}