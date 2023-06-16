using Microsoft.AspNetCore.Mvc;
using Pagination.EntityFrameworkCore.Extensions;
using SocialMediaApi.Domain.Models.PostComments;
using SocialMediaApi.Domain.ViewModels;
using SocialMediaApi.Interfaces;
using SocialMediaApi.Interfaces.UnitOfWork;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SocialMediaApi.Controllers
{
    [Route("api/v1/post/{postId}/comments")]
    [ApiController]
    public class PostCommentsController : ControllerBase
    {
        private readonly IPostCommentService _postService;

        public PostCommentsController(IPostUnitOfWork postUnitOfWork)
        {
            _postService = postUnitOfWork.PostCommentService;
        }

        [HttpGet]
        public async Task<ActionResult<Pagination<PostCommentViewModel>>> GetPostCommentsAsync([FromRoute] Guid postId, int page = 1, int limit = 20)
        {
            if (limit <= 0)
            {
                limit = 20;
            }

            return Ok(await _postService.GetPostCommentsAsync(postId, page, limit));
        }

        [HttpPost]
        public async Task<ActionResult<PostCommentViewModel>> AddPostCommentAsync([FromRoute] Guid postId, [FromBody] AddPostCommentModel model)
        {
            return Ok(await _postService.AddPostCommentAsync(postId, model));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<PostCommentViewModel>> UpdatePostCommentAsync([FromRoute] Guid postId, [FromRoute] Guid id, [FromBody] UpdatePostCommentModel model)
        {
            return Ok(await _postService.UpdatePostCommentAsync(postId, id, model));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePostCommentAsync([FromRoute] Guid postId, [FromRoute] Guid id)
        {
            await _postService.DeletePostCommentAsync(postId, id);
            return Ok();
        }
    }
}