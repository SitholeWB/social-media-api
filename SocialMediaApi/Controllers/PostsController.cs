using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pagination.EntityFrameworkCore.Extensions;
using SocialMediaApi.Domain.Models.Posts;
using SocialMediaApi.Domain.ViewModels;
using SocialMediaApi.Interfaces;
using SocialMediaApi.Interfaces.UnitOfWork;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SocialMediaApi.Controllers
{
    [Authorize]
    [Route("api/v1/groups/{groupId}/posts")]
    [ApiController]
    public class PostsController : ControllerBase
    {
        private readonly IPostService _postService;
        private readonly IActivePostService _activePostService;

        public PostsController(IPostUnitOfWork postUnitOfWork)
        {
            _postService = postUnitOfWork.PostService;
            _activePostService = postUnitOfWork.ActivePostService;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<Pagination<PostViewModel>>> GetPostsAsync([FromRoute] Guid groupId, int page = 1, int limit = 20, bool skipActivePosts = false)
        {
            if (limit <= 0)
            {
                limit = 20;
            }
            if (skipActivePosts)
            {
                return Ok(await _postService.GetPostsAsync(groupId, page, limit));
            }
            var pageResult = await _activePostService.GetActivePostsAsync(groupId, page, limit);
            if (pageResult.Results.Count() < (limit / 2))
            {
                pageResult = await _postService.GetPostsAsync(groupId, page, limit);
            }
            return Ok(pageResult);
        }

        [AllowAnonymous]
        [HttpGet("{id}")]
        public async Task<ActionResult<PostViewModel>> GetPostAsync([FromRoute] Guid groupId, [FromRoute] Guid id)
        {
            return Ok(await _postService.GetPostAsync(groupId, id));
        }

        [HttpPost]
        public async Task<ActionResult<PostViewModel>> AddPostAsync([FromRoute] Guid groupId, [FromBody] AddPostModel model)
        {
            return Ok(await _postService.AddPostAsync(groupId, model));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<PostViewModel>> UpdatePostAsync([FromRoute] Guid groupId, [FromRoute] Guid id, [FromBody] UpdatePostModel model)
        {
            return Ok(await _postService.UpdatePostAsync(groupId, id, model));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePostAsync([FromRoute] Guid groupId, [FromRoute] Guid id)
        {
            await _postService.DeletePostAsync(groupId, id);
            return Ok();
        }
    }
}