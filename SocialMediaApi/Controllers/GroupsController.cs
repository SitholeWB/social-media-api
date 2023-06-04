using Microsoft.AspNetCore.Mvc;
using Pagination.EntityFrameworkCore.Extensions;
using SocialMediaApi.Domain.Models.Group;
using SocialMediaApi.Domain.ViewModels;
using SocialMediaApi.Interfaces;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SocialMediaApi.Controllers
{
    [Route("api/v1/groups")]
    [ApiController]
    public class GroupsController : ControllerBase
    {
        private readonly IGroupService _groupService;

        public GroupsController(IGroupService groupService)
        {
            _groupService = groupService;
        }

        [HttpGet]
        public async Task<ActionResult<Pagination<GroupViewModel>>> GetGroupsAsync(int page = 1)
        {
            var tt = await _groupService.GetGroupsAsync(page, 20);
            return Ok(tt);
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<GroupViewModel>> GetGroupAsync(Guid id)
        {
            return Ok(await _groupService.GetGroupAsync(id));
        }

        [HttpPost]
        public async Task<ActionResult<GroupViewModel>> AddGroupAsync([FromBody] AddGroupModel model)
        {
            return Ok(await _groupService.AddGroupAsync(model));
        }

        [HttpPut("{id}")]
        public async Task<ActionResult<GroupViewModel>> UpdateGroupAsync(Guid id, [FromBody] UpdateGroupModel model)
        {
            return Ok(await _groupService.UpdateGroupAsync(id, model));
        }

        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteGroupAsync(Guid id)
        {
            await _groupService.DeleteGroupAsync(id);
            return Ok();
        }
    }
}