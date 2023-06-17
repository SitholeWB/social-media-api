using Microsoft.AspNetCore.Mvc;
using Pagination.EntityFrameworkCore.Extensions;
using SocialMediaApi.Domain.ViewModels;
using SocialMediaApi.Interfaces;
using SocialMediaApi.Interfaces.UnitOfWork;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SocialMediaApi.Controllers
{
    [Route("api/v1/users/{userId}/posts")]
    [ApiController]
    public class UserPostsController : ControllerBase
    {
        private readonly IUserPostService _postService;

        public UserPostsController(IPostUnitOfWork postUnitOfWork)
        {
            _postService = postUnitOfWork.UserPostService;
        }

        [HttpGet]
        public async Task<ActionResult<Pagination<PostViewModel>>> GetPostsAsync([FromRoute] Guid userId, int page = 1)
        {
            return Ok(await _postService.GetUserPostsAsync(userId, page));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePostAsync([FromRoute] Guid userId, [FromRoute] Guid id)
        {
            await _postService.DeleteUserPostAsync(userId, id);
            return Ok();
        }
    }
}