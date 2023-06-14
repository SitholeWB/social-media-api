using Microsoft.AspNetCore.Mvc;
using Pagination.EntityFrameworkCore.Extensions;
using SocialMediaApi.Domain.Models.GroupPostComments;
using SocialMediaApi.Domain.ViewModels;
using SocialMediaApi.Interfaces;
using SocialMediaApi.Interfaces.UnitOfWork;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SocialMediaApi.Controllers
{
    [Route("api/v1/groups/{groupId}/posts/{postId}/comments")]
    [ApiController]
    public class GroupPostCommentsController : ControllerBase
    {
        private readonly IGroupPostCommentService _groupPostService;

        public GroupPostCommentsController(IPostUnitOfWork postUnitOfWork)
        {
            _groupPostService = postUnitOfWork.GroupPostCommentService;
        }

        [HttpGet]
        public async Task<ActionResult<Pagination<GroupPostCommentViewModel>>> GetGroupPostCommentsAsync([FromRoute] Guid groupId, [FromRoute] Guid postId, int page = 1, int limit = 20)
        {
            if (limit <= 0)
            {
                limit = 20;
            }

            return Ok(await _groupPostService.GetGroupPostCommentsAsync(postId, page, limit));
        }

        [HttpPost]
        public async Task<ActionResult<GroupPostCommentViewModel>> AddGroupPostCommentAsync([FromRoute] Guid groupId, [FromRoute] Guid postId, [FromBody] AddGroupPostCommentModel model)
        {
            return Ok(await _groupPostService.AddGroupPostCommentAsync(postId, model));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<GroupPostCommentViewModel>> UpdateGroupPostCommentAsync([FromRoute] Guid groupId, [FromRoute] Guid postId, [FromRoute] Guid id, [FromBody] UpdateGroupPostCommentModel model)
        {
            return Ok(await _groupPostService.UpdateGroupPostCommentAsync(postId, id, model));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGroupPostCommentAsync([FromRoute] Guid groupId, [FromRoute] Guid postId, [FromRoute] Guid id)
        {
            await _groupPostService.DeleteGroupPostCommentAsync(postId, id);
            return Ok();
        }
    }
}