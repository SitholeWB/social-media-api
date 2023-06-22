using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pagination.EntityFrameworkCore.Extensions;
using SocialMediaApi.Domain.ViewModels;
using SocialMediaApi.Interfaces;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SocialMediaApi.Controllers
{
    [Authorize]
    [Route("api/v1/feed")]
    [ApiController]
    public class FeedController : ControllerBase
    {
        private readonly IFeedService _feedService;

        public FeedController(IFeedService feedService)
        {
            _feedService = feedService;
        }

        [HttpGet]
        public async Task<ActionResult<Pagination<PostViewModel>>> GetPostsAsync(int page = 1, int limit = 20, bool skipActivePosts = false)
        {
            return Ok(await _feedService.GetPostsAsync(page, limit, skipActivePosts));
        }

        [AllowAnonymous]
        [HttpGet("trending")]
        public async Task<ActionResult<Pagination<PostViewModel>>> GetTrendingPostsAsync(int page = 1, int limit = 20)
        {
            return Ok(await _feedService.GetTrendingPostsAsync(page, limit));
        }
    }
}