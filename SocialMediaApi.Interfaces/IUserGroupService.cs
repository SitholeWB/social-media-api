using Pagination.EntityFrameworkCore.Extensions;
using SocialMediaApi.Domain.Models.UserGroups;
using SocialMediaApi.Domain.ViewModels;

namespace SocialMediaApi.Interfaces
{
    public interface IUserGroupService
    {
        public Task AddUserGroupAsync(AddUserGroupModel model);

        public Task DeleteUserGroupAsync(Guid entityId);

        public Task<Pagination<GroupViewModel>> GetUserGroupsAsync();
    }
}