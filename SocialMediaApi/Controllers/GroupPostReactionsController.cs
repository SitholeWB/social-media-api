using Microsoft.AspNetCore.Mvc;
using SocialMediaApi.Domain.Models.Reactions;
using SocialMediaApi.Domain.ViewModels;
using SocialMediaApi.Interfaces;
using SocialMediaApi.Interfaces.UnitOfWork;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SocialMediaApi.Controllers
{
    [Route("api/v1/post/{postId}/reactions")]
    [ApiController]
    public class GroupPostReactionsController : ControllerBase
    {
        private readonly IPostReactionService _postReactionService;
        private readonly IEntityDetailsService _entityDetailsService;

        public GroupPostReactionsController(IPostUnitOfWork postUnitOfWork)
        {
            _postReactionService = postUnitOfWork.PostReactionService;
            _entityDetailsService = postUnitOfWork.EntityDetailsService;
        }

        [HttpGet]
        public async Task<ActionResult<EntityReactionViewModel>> GetReactionAsync([FromRoute] Guid postId)
        {
            return Ok(await _entityDetailsService.GetReactionAsync(postId));
        }

        [HttpPost]
        public async Task<ActionResult<GroupPostCommentViewModel>> AddReactionAsync([FromRoute] Guid postId, [FromBody] AddEntityReactionModel model)
        {
            return Ok(await _postReactionService.AddReactionAsync(model));
        }

        [HttpDelete]
        public async Task<ActionResult<GroupPostCommentViewModel>> DeleteReactionAsync([FromRoute] Guid postId)
        {
            return Ok(await _postReactionService.DeleteReactionAsync(postId));
        }
    }
}