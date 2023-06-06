using Microsoft.AspNetCore.Mvc;
using Pagination.EntityFrameworkCore.Extensions;
using SocialMediaApi.Domain.Models.GroupPosts;
using SocialMediaApi.Domain.ViewModels;
using SocialMediaApi.Interfaces;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SocialMediaApi.Controllers
{
    [Route("api/v1/groups/{groupId}/posts")]
    [ApiController]
    public class GroupPostsController : ControllerBase
    {
        private readonly IGroupPostService _groupPostService;

        public GroupPostsController(IGroupPostService groupPostService)
        {
            _groupPostService = groupPostService;
        }

        [HttpGet]
        public async Task<ActionResult<Pagination<GroupPostViewModel>>> GetGroupPostsAsync([FromRoute] Guid groupId, int page = 1)
        {
            return Ok(await _groupPostService.GetGroupPostsAsync(groupId, page, 20));
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<GroupPostViewModel>> GetGroupPostAsync([FromRoute] Guid groupId, [FromRoute] Guid id)
        {
            return Ok(await _groupPostService.GetGroupPostAsync(groupId, id));
        }

        [HttpPost]
        public async Task<ActionResult<GroupPostViewModel>> AddGroupPostAsync([FromRoute] Guid groupId, [FromBody] AddGroupPostModel model)
        {
            return Ok(await _groupPostService.AddGroupPostAsync(groupId, model));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<GroupPostViewModel>> UpdateGroupPostAsync([FromRoute] Guid groupId, [FromRoute] Guid id, [FromBody] UpdateGroupPostModel model)
        {
            return Ok(await _groupPostService.UpdateGroupPostAsync(groupId, id, model));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGroupPostAsync([FromRoute] Guid groupId, [FromRoute] Guid id)
        {
            await _groupPostService.DeleteGroupPostAsync(groupId, id);
            return Ok();
        }
    }
}