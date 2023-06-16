using Microsoft.AspNetCore.Mvc;
using Pagination.EntityFrameworkCore.Extensions;
using SocialMediaApi.Domain.Models.Comments;
using SocialMediaApi.Domain.ViewModels;
using SocialMediaApi.Interfaces;
using SocialMediaApi.Interfaces.UnitOfWork;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SocialMediaApi.Controllers
{
    [Route("api/v1/post/{postId}/comments")]
    [ApiController]
    public class CommentsController : ControllerBase
    {
        private readonly ICommentService _postService;

        public CommentsController(IPostUnitOfWork postUnitOfWork)
        {
            _postService = postUnitOfWork.CommentService;
        }

        [HttpGet]
        public async Task<ActionResult<Pagination<CommentViewModel>>> GetCommentsAsync([FromRoute] Guid postId, int page = 1, int limit = 20)
        {
            if (limit <= 0)
            {
                limit = 20;
            }

            return Ok(await _postService.GetCommentsAsync(postId, page, limit));
        }

        [HttpPost]
        public async Task<ActionResult<CommentViewModel>> AddCommentAsync([FromRoute] Guid postId, [FromBody] AddCommentModel model)
        {
            return Ok(await _postService.AddCommentAsync(postId, model));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<CommentViewModel>> UpdateCommentAsync([FromRoute] Guid postId, [FromRoute] Guid id, [FromBody] UpdateCommentModel model)
        {
            return Ok(await _postService.UpdateCommentAsync(postId, id, model));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCommentAsync([FromRoute] Guid postId, [FromRoute] Guid id)
        {
            await _postService.DeleteCommentAsync(postId, id);
            return Ok();
        }
    }
}