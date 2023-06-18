using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using SocialMediaApi.Domain.Models.Posts;
using SocialMediaApi.Domain.ViewModels;
using SocialMediaApi.Interfaces;
using SocialMediaApi.Interfaces.UnitOfWork;

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