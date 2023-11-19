using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialMediaApi.Domain.Interfaces;
using SocialMediaApi.Domain.Interfaces.UnitOfWork;
using SocialMediaApi.Domain.Models.Posts;
using SocialMediaApi.Domain.ViewModels;
using SocialMediaApi.Extensions;

namespace SocialMediaApi.Controllers
{
	[Authorize]
	public abstract class BasePostsController : ControllerBase
	{
		private readonly IPostService _postService;

		protected BasePostsController(IPostUnitOfWork postUnitOfWork)
		{
			_postService = postUnitOfWork.PostService;
		}

		[AllowAnonymous]
		[HttpGet("{id}")]
		public async Task<ActionResult<PostViewModel>> GetPostAsync([FromRoute] Guid ownerId, [FromRoute] Guid id)
		{
			return Ok(await _postService.GetPostAsync(this.GetAuthUser(), ownerId, id));
		}

		[HttpPost]
		public async Task<ActionResult<PostViewModel>> AddPostAsync([FromRoute] Guid ownerId, [FromBody] AddPostModel model)
		{
			return Ok(await _postService.AddPostAsync(this.GetAuthUser(), ownerId, model));
		}

		[HttpPut("{id}")]
		public async Task<ActionResult<PostViewModel>> UpdatePostAsync([FromRoute] Guid ownerId, [FromRoute] Guid id, [FromBody] UpdatePostModel model)
		{
			return Ok(await _postService.UpdatePostAsync(this.GetAuthUser(), ownerId, id, model));
		}

		[HttpDelete("{id}")]
		public async Task<IActionResult> DeletePostAsync([FromRoute] Guid ownerId, [FromRoute] Guid id)
		{
			await _postService.DeletePostAsync(this.GetAuthUser(), ownerId, id);
			return Ok();
		}
	}
}