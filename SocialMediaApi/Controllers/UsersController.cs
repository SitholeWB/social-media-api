using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialMediaApi.Domain.DTOs;
using SocialMediaApi.Domain.Models.JwtTokens;
using SocialMediaApi.Domain.Models.Users;
using SocialMediaApi.Domain.ViewModels;
using SocialMediaApi.Interfaces;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SocialMediaApi.Controllers
{
    [Route("api/v1/users")]
    [ApiController]
    public class UsersController : ControllerBase
    {
        private readonly IUserService _personService;
        private readonly IAuthService _tokenService;

        public UsersController(IUserService personService, IAuthService tokenService)
        {
            _personService = personService;
            _tokenService = tokenService;
        }

        [AllowAnonymous]
        [HttpPost("register")]
        public async Task<ActionResult<UserViewModel>> AddPerson(AddUserModel model)
        {
            return Ok(await _personService.AddUserAsync(model));
        }

        [AllowAnonymous]
        [HttpPost("token")]
        public async Task<ActionResult<TokenDto>> GetToken(AddJwtTokenModel entity)
        {
            return Ok(await _tokenService.BuildToken(entity));
        }
    }
}