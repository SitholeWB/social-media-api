// Ignore Spelling: Auth

using SocialMediaApi.Domain.DTOs;
using SocialMediaApi.Domain.Models.Security;

namespace SocialMediaApi.Domain.Interfaces
{
	public interface IAuthService
	{
		Task<TokenDto> BuildToken(JwtTokenModel model);
	}
}