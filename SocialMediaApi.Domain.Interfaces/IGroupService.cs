using Pagination.EntityFrameworkCore.Extensions;
using SocialMediaApi.Domain.Models.Groups;
using SocialMediaApi.Domain.Models.Security;
using SocialMediaApi.Domain.ViewModels;

namespace SocialMediaApi.Domain.Interfaces
{
	public interface IGroupService
	{
		public Task<GroupViewModel> AddGroupAsync(AuthUser authUser, AddGroupModel model);

		public Task<GroupViewModel> UpdateGroupAsync(AuthUser authUser, Guid id, UpdateGroupModel model);

		public Task DeleteGroupAsync(AuthUser authUser, Guid id);

		public Task<GroupViewModel?> GetGroupAsync(Guid id);

		public Task<Pagination<GroupViewModel>> GetGroupsAsync(int page = 1, int limit = 20);
	}
}