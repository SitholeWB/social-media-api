using Microsoft.IdentityModel.Tokens;
using SocialMediaApi.Domain.Common;
using SocialMediaApi.Domain.DTOs;
using SocialMediaApi.Domain.Exceptions;
using SocialMediaApi.Domain.Interfaces;
using SocialMediaApi.Domain.Models.Security;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SocialMediaApi.Domain.Logic.Services
{
	public class AuthService : IAuthService
	{
		private readonly IConfigService _configService;
		private readonly IUserService _userService;

		public AuthService(IConfigService configService, IUserService userService)
		{
			_configService = configService;
			_userService = userService;
		}

		public async Task<TokenDto> BuildToken(JwtTokenModel model)
		{
			if (string.IsNullOrWhiteSpace(model.Email))
			{
				throw new SocialMediaException("Email is required.");
			}
			if (string.IsNullOrWhiteSpace(model.Password))
			{
				throw new SocialMediaException("Password is required.");
			}

			var person = await _userService.GetUserByEmailAsync(model.Email.Trim());
			if (person == null)
			{
				throw new SocialMediaException("Given email or password is incorrect");
			}

			var inputPassword = HashingUtils.HashUserPassword(person.Id, model.Password);
			if (!inputPassword.Equals(person.Password))
			{
				throw new SocialMediaException("Given email/user-name/password is incorrect");
			}
			//if (!person.IsApproved)
			//{
			//	throw new PostException("User is not approved.");
			//}
			var claims = new[]
			{
				new Claim(type: ClaimTypes.Name, person.FirstName),
				new Claim(type : ClaimTypes.Surname, person.LastName),
				new Claim(type : ClaimTypes.NameIdentifier, person.Id.ToString())
			 };
			var jwtSettings = await _configService.GetJwtConfigAsync();
			var key = jwtSettings.IssuerSigningKey;
			var issuer = jwtSettings.ValidIssuer;
			var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(key));
			var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256Signature);
			var tokenDescriptor = new JwtSecurityToken(issuer, issuer, claims, expires: DateTime.Now.AddMonths(12), signingCredentials: credentials);
			return new TokenDto
			{
				Token = new JwtSecurityTokenHandler().WriteToken(tokenDescriptor),
				ExpireOn = tokenDescriptor.ValidTo,
				Name = person.FirstName,
				Surname = person.LastName,
			};
		}
	}
}