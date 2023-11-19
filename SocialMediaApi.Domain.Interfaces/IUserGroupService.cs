using Pagination.EntityFrameworkCore.Extensions;
using SocialMediaApi.Domain.Models.Security;
using SocialMediaApi.Domain.Models.UserGroups;
using SocialMediaApi.Domain.ViewModels;

namespace SocialMediaApi.Domain.Interfaces
{
	public interface IUserGroupService
	{
		public Task AddUserGroupAsync(AuthUser authUser, AddUserGroupModel model);

		public Task DeleteUserGroupAsync(AuthUser authUser, Guid entityId);

		public Task<Pagination<GroupViewModel>> GetUserGroupsAsync(AuthUser authUser);
	}
}