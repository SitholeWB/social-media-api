using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialMediaApi.Domain.Interfaces;
using SocialMediaApi.Domain.Interfaces.UnitOfWork;
using SocialMediaApi.Domain.Models.Reactions;
using SocialMediaApi.Domain.ViewModels;
using SocialMediaApi.Extensions;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SocialMediaApi.Controllers
{
	[Authorize]
	[Route("api/v1/comment/{commentId}/reactions")]
	[ApiController]
	public class CommentReactionsController : ControllerBase
	{
		private readonly ICommentReactionService _commentReactionService;
		private readonly IEntityDetailsService _entityDetailsService;

		public CommentReactionsController(IPostUnitOfWork postUnitOfWork)
		{
			_commentReactionService = postUnitOfWork.CommentReactionService;
			_entityDetailsService = postUnitOfWork.EntityDetailsService;
		}

		[AllowAnonymous]
		[HttpGet]
		public async Task<ActionResult<EntityReactionViewModel>> GetReactionAsync([FromRoute] Guid commentId)
		{
			return Ok(await _entityDetailsService.GetReactionAsync(commentId));
		}

		[HttpPost]
		public async Task<ActionResult<CommentViewModel>> AddReactionAsync([FromRoute] Guid commentId, [FromBody] AddEntityReactionModel model)
		{
			return Ok(await _commentReactionService.AddReactionAsync(this.GetAuthUser(), commentId, model));
		}

		[HttpDelete]
		public async Task<ActionResult<CommentViewModel>> DeleteReactionAsync([FromRoute] Guid commentId)
		{
			return Ok(await _commentReactionService.DeleteReactionAsync(this.GetAuthUser(), commentId));
		}
	}
}