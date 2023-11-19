using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pagination.EntityFrameworkCore.Extensions;
using SocialMediaApi.Domain.DTOs;
using SocialMediaApi.Domain.Interfaces;
using SocialMediaApi.Domain.Interfaces.UnitOfWork;
using SocialMediaApi.Domain.Models.Posts;
using SocialMediaApi.Domain.Models.Security;
using SocialMediaApi.Domain.Models.UserGroups;
using SocialMediaApi.Domain.Models.Users;
using SocialMediaApi.Domain.ViewModels;
using SocialMediaApi.Extensions;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SocialMediaApi.Controllers
{
	[Route("api/v1/users")]
	[ApiController]
	public class UsersController : ControllerBase
	{
		private readonly IUserService _personService;
		private readonly IAuthService _tokenService;
		private readonly IUserGroupService _userGroupService;
		private readonly IPostService _postService;
		private readonly IUserPostService _userPostService;

		public UsersController(IUserService personService, IAuthService tokenService, IPostUnitOfWork postUnitOfWork)
		{
			_personService = personService;
			_tokenService = tokenService;
			_userGroupService = postUnitOfWork.UserGroupService;
			_postService = postUnitOfWork.PostService;
			_userPostService = postUnitOfWork.UserPostService;
		}

		[AllowAnonymous]
		[HttpPost("register")]
		public async Task<ActionResult<UserViewModel>> AddPerson(AddUserModel model)
		{
			return Ok(await _personService.AddUserAsync(model));
		}

		[AllowAnonymous]
		[HttpPost("token")]
		public async Task<ActionResult<TokenDto>> GetToken(JwtTokenModel model)
		{
			return Ok(await _tokenService.BuildToken(model));
		}

		[HttpPost("groups")]
		public async Task<IActionResult> AddUserGroupAsync([FromBody] AddUserGroupModel model)
		{
			await _userGroupService.AddUserGroupAsync(this.GetAuthUser(), model);
			return Ok();
		}

		[HttpGet("groups")]
		public async Task<ActionResult<Pagination<GroupViewModel>>> GetUserGroupsAsync()
		{
			return Ok(await _userGroupService.GetUserGroupsAsync(this.GetAuthUser()));
		}

		[HttpDelete("groups/{id}")]
		public async Task<IActionResult> DeleteReactionAsync([FromRoute] Guid id)
		{
			await _userGroupService.DeleteUserGroupAsync(this.GetAuthUser(), id);
			return Ok();
		}

		// Posts
		[AllowAnonymous]
		[HttpGet("posts")]
		public async Task<ActionResult<Pagination<PostViewModel>>> GetPostsAsync(int page = 1)
		{
			var authUser = this.GetAuthUser();
			return Ok(await _userPostService.GetUserPostsAsync(authUser, authUser.AuthorizedUser.Id, page));
		}

		[AllowAnonymous]
		[HttpGet("posts/{id}")]
		public async Task<ActionResult<PostViewModel>> GetPostAsync([FromRoute] Guid id)
		{
			var authUser = this.GetAuthUser();
			return Ok(await _postService.GetPostAsync(authUser, authUser.AuthorizedUser.Id, id));
		}

		[HttpPost("posts")]
		public async Task<ActionResult<PostViewModel>> AddPostAsync([FromBody] AddPostModel model)
		{
			var authUser = this.GetAuthUser();
			return Ok(await _postService.AddPostAsync(authUser, authUser.AuthorizedUser.Id, model));
		}

		[HttpPut("posts/{id}")]
		public async Task<ActionResult<PostViewModel>> UpdatePostAsync([FromRoute] Guid id, [FromBody] UpdatePostModel model)
		{
			var authUser = this.GetAuthUser();
			return Ok(await _postService.UpdatePostAsync(authUser, authUser.AuthorizedUser.Id, id, model));
		}

		[HttpDelete("posts/{id}")]
		public async Task<IActionResult> DeletePostAsync([FromRoute] Guid id)
		{
			var authUser = this.GetAuthUser();
			await _postService.DeletePostAsync(authUser, authUser.AuthorizedUser.Id, id);
			return Ok();
		}
	}
}