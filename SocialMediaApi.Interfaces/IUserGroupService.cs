using SocialMediaApi.Domain.Entities.JsonEntities;
using SocialMediaApi.Domain.Models.UserGroups;

namespace SocialMediaApi.Interfaces
{
    public interface IUserGroupService
    {
        public Task AddUserGroupAsync(AddUserGroupModel model);

        public Task DeleteUserGroupAsync(Guid entityId);

        public Task<IList<MiniEntity>> GetUserGroupsAsync();
    }
}