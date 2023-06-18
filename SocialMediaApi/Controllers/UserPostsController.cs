using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pagination.EntityFrameworkCore.Extensions;
using SocialMediaApi.Domain.ViewModels;
using SocialMediaApi.Interfaces;
using SocialMediaApi.Interfaces.UnitOfWork;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SocialMediaApi.Controllers
{
    [Authorize]
    [Route("api/v1/users/{userId}/posts")]
    [ApiController]
    public class UserPostsController : BasePostsController
    {
        private readonly IUserPostService _postService;

        public UserPostsController(IPostUnitOfWork postUnitOfWork) : base(postUnitOfWork)
        {
            _postService = postUnitOfWork.UserPostService;
        }

        [AllowAnonymous]
        [HttpGet]
        public async Task<ActionResult<Pagination<PostViewModel>>> GetPostsAsync([FromRoute] Guid userId, int page = 1)
        {
            return Ok(await _postService.GetUserPostsAsync(userId, page));
        }
    }
}