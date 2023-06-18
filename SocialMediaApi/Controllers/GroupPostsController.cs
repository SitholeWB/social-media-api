using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pagination.EntityFrameworkCore.Extensions;
using SocialMediaApi.Domain.ViewModels;
using SocialMediaApi.Interfaces;
using SocialMediaApi.Interfaces.UnitOfWork;

namespace SocialMediaApi.Controllers
{
    [Authorize]
    [Route("api/v1/groups/{groupId}/posts")]
    [ApiController]
    public class GroupPostsController : BasePostsController
    {
        private readonly IPostService _postService;
        private readonly IActivePostService _activePostService;
        public GroupPostsController(IPostUnitOfWork postUnitOfWork) : base(postUnitOfWork)
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
    }
}