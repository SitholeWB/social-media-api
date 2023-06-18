using Microsoft.AspNetCore.Http;
using Microsoft.IdentityModel.Tokens;
using SocialMediaApi.Domain.Common;
using SocialMediaApi.Domain.DTOs;
using SocialMediaApi.Domain.Entities.Base;
using SocialMediaApi.Domain.Exceptions;
using SocialMediaApi.Domain.Models.JwtTokens;
using SocialMediaApi.Interfaces;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace SocialMediaApi.Logic.Services
{
    public class AuthService : IAuthService
    {
        private readonly IConfigService _configService;
        private readonly IUserService _userService;
        private readonly IHttpContextAccessor _httpContextAccessor;

        public AuthService(IConfigService configService, IUserService userService, IHttpContextAccessor httpContextAccessor)
        {
            _configService = configService;
            _userService = userService;
            _httpContextAccessor = httpContextAccessor;
        }

        public async Task<TokenDto> BuildToken(AddJwtTokenModel model)
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

        public async Task<BaseUser> GetAuthenticatedUser()
        {
            return await Task.FromResult(new BaseUser
            {
                ImageUrl = "To be implemented.",
                Name = "To be implemented."
            });
        }

        public async Task<BaseUser> GetAuthorizedUser()
        {
            var isAuthenticated = _httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false;
            var installationId = GetInstallationId();
            if (!isAuthenticated)
            {
                return await Task.FromResult(new BaseUser
                {
                    Id = Guid.Empty,
                    Name = GetAnonymousName(),
                    InstallationId = installationId
                });
            }
            var userId = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.NameIdentifier)?.Value ?? Guid.Empty.ToString();
            var name = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Name)?.Value ?? "Anonymous Person";
            var surname = _httpContextAccessor.HttpContext?.User?.FindFirst(ClaimTypes.Surname)?.Value ?? string.Empty;
            Guid.TryParse(userId, out Guid userIdValue);
            return await Task.FromResult(new BaseUser
            {
                Id = userIdValue,
                Name = $"{name} {surname}",
                InstallationId = installationId
            });
        }

        public async Task<BaseUser> GetImpersonatingUser()
        {
            return await Task.FromResult(new BaseUser
            {
                ImageUrl = "To be implemented.",
                Name = "To be implemented."
            });
        }

        public string GetAnonymousName()
        {
            return (_httpContextAccessor?.HttpContext?.Request?.Headers["x-anonymous-name"] ?? "Anonymous")!;
        }

        public string GetUserId()
        {
            return (_httpContextAccessor?.HttpContext?.Request?.Headers["x-user-id-header"] ?? string.Empty)!;
        }

        public string GetInstallationId()
        {
            return (_httpContextAccessor?.HttpContext?.Request?.Headers["x-installation-id-header"] ?? string.Empty)!;
        }

        public async Task<bool> IsAuthenticated()
        {
            return await Task.FromResult(_httpContextAccessor.HttpContext?.User?.Identity?.IsAuthenticated ?? false);
        }
    }
}