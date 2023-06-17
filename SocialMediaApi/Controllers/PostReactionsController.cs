using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialMediaApi.Domain.Models.Reactions;
using SocialMediaApi.Domain.ViewModels;
using SocialMediaApi.Interfaces;
using SocialMediaApi.Interfaces.UnitOfWork;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SocialMediaApi.Controllers
{
    [Authorize]
    [Route("api/v1/post/{postId}/reactions")]
    [ApiController]
    public class PostReactionsController : ControllerBase
    {
        private readonly IPostReactionService _postReactionService;
        private readonly IEntityDetailsService _entityDetailsService;

        public PostReactionsController(IPostUnitOfWork postUnitOfWork)
        {
            _postReactionService = postUnitOfWork.PostReactionService;
            _entityDetailsService = postUnitOfWork.EntityDetailsService;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<EntityReactionViewModel>> GetReactionAsync([FromRoute] Guid postId)
        {
            return Ok(await _entityDetailsService.GetReactionAsync(postId));
        }

        [HttpPost]
        public async Task<ActionResult<CommentViewModel>> AddReactionAsync([FromRoute] Guid postId, [FromBody] AddEntityReactionModel model)
        {
            return Ok(await _postReactionService.AddReactionAsync(postId, model));
        }

        [HttpDelete]
        public async Task<ActionResult<CommentViewModel>> DeleteReactionAsync([FromRoute] Guid postId)
        {
            return Ok(await _postReactionService.DeleteReactionAsync(postId));
        }
    }
}