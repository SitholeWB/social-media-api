﻿using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Pagination.EntityFrameworkCore.Extensions;
using SocialMediaApi.Domain.Interfaces;
using SocialMediaApi.Domain.Models.Groups;
using SocialMediaApi.Domain.ViewModels;
using SocialMediaApi.Extensions;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SocialMediaApi.Controllers
{
	[Authorize]
	[Route("api/v1/groups")]
	[ApiController]
	public class GroupsController : ControllerBase
	{
		private readonly IGroupService _groupService;

		public GroupsController(IGroupService groupService)
		{
			_groupService = groupService;
		}

		[AllowAnonymous]
		[HttpGet]
		public async Task<ActionResult<Pagination<GroupViewModel>>> GetGroupsAsync(int page = 1)
		{
			return Ok(await _groupService.GetGroupsAsync(page, 20));
		}

		[AllowAnonymous]
		[HttpGet("{id}")]
		public async Task<ActionResult<GroupViewModel>> GetGroupAsync(Guid id)
		{
			return Ok(await _groupService.GetGroupAsync(id));
		}

		[HttpPost]
		public async Task<ActionResult<GroupViewModel>> AddGroupAsync([FromBody] AddGroupModel model)
		{
			return Ok(await _groupService.AddGroupAsync(this.GetAuthUser(), model));
		}

		[HttpPut("{id}")]
		public async Task<ActionResult<GroupViewModel>> UpdateGroupAsync(Guid id, [FromBody] UpdateGroupModel model)
		{
			return Ok(await _groupService.UpdateGroupAsync(this.GetAuthUser(), id, model));
		}

		[HttpDelete("{id}")]
		public async Task<IActionResult> DeleteGroupAsync(Guid id)
		{
			await _groupService.DeleteGroupAsync(this.GetAuthUser(), id);
			return Ok();
		}
	}
}