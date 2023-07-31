using Pagination.EntityFrameworkCore.Extensions;
using SocialMediaApi.Domain.Models.Groups;
using SocialMediaApi.Domain.ViewModels;

namespace SocialMediaApi.Domain.Interfaces
{
    public interface IGroupService
    {
        public Task<GroupViewModel> AddGroupAsync(AddGroupModel model);

        public Task<GroupViewModel> UpdateGroupAsync(Guid id, UpdateGroupModel model);

        public Task DeleteGroupAsync(Guid id);

        public Task<GroupViewModel?> GetGroupAsync(Guid id);

        public Task<Pagination<GroupViewModel>> GetGroupsAsync(int page = 1, int limit = 20);
    }
}