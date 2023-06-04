using Microsoft.EntityFrameworkCore;
using Pagination.EntityFrameworkCore.Extensions;
using SocialMediaApi.Domain.Entities;
using SocialMediaApi.Domain.Enums;
using SocialMediaApi.Domain.Exceptions;
using SocialMediaApi.Domain.Mappers;
using SocialMediaApi.Domain.Models.Group;
using SocialMediaApi.Domain.ViewModels;
using SocialMediaApi.Interfaces;
using SocialMediaApi.Repositories;

namespace SocialMediaApi.Services
{
    public class GroupService : IGroupService
    {
        private readonly SocialMediaApiDbContext _dbContext;
        private readonly IAuthService _authService;

        public GroupService(SocialMediaApiDbContext dbContext, IAuthService authService)
        {
            _dbContext = dbContext;
            _authService = authService;
        }

        public async Task<GroupViewModel> AddGroupAsync(AddGroupModel model)
        {
            if (string.IsNullOrEmpty(model?.Name))
            {
                throw new SocialMediaException("Group name is required.");
            }
            var authUser = await _authService.GetAuthorizedUser();
            var entity = new Group
            {
                Name = model.Name,
                CreatedDate = DateTimeOffset.UtcNow,
                Description = model.Description,
                Id = Guid.NewGuid(),
                EntityStatus = EntityStatus.Ready,
                LastModifiedDate = DateTimeOffset.UtcNow,
                Creator = authUser,
            };
            var addedEntity = await _dbContext.AddAsync(entity);
            await _dbContext.SaveChangesAsync();
            return GroupMapper.ToView(addedEntity.Entity)!;
        }

        public async Task DeleteGroupAsync(Guid id)
        {
            var group = await _dbContext.Groups.FirstOrDefaultAsync(x => x.Id == id) ?? throw new SocialMediaException("No Group found for given Id.");
            var authUser = await _authService.GetAuthorizedUser();
            if (!authUser.Id.Equals(group.Creator.Id))
            {
                throw new SocialMediaException("Group can only be deleted by the creator.");
            }
            _dbContext.Groups.Remove(group);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<GroupViewModel?> GetGroupAsync(Guid id)
        {
            return GroupMapper.ToView(await _dbContext.Groups.FirstOrDefaultAsync(x => x.Id == id));
        }

        public async Task<Pagination<GroupViewModel>> GetGroupsAsync(int page = 1, int limit = 20)
        {
            return await _dbContext.AsPaginationAsync<Group, GroupViewModel>(page, limit, GroupMapper.ToView!);
        }

        public async Task<GroupViewModel> UpdateGroupAsync(Guid id, UpdateGroupModel model)
        {
            if (string.IsNullOrEmpty(model?.Name))
            {
                throw new SocialMediaException("Group name is required.");
            }
            var group = await _dbContext.Groups.FirstOrDefaultAsync(x => x.Id == id) ?? throw new SocialMediaException("No Group found for given Id.");

            group.Name = model.Name;
            group.Description = model.Description;
            group.LastModifiedDate = DateTimeOffset.UtcNow;
            _dbContext.Update(group);
            await _dbContext.SaveChangesAsync();
            return GroupMapper.ToView(group)!;
        }
    }
}