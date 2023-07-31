using Microsoft.EntityFrameworkCore;
using Pagination.EntityFrameworkCore.Extensions;
using SocialMediaApi.Data;
using SocialMediaApi.Domain.Entities;
using SocialMediaApi.Domain.Enums;
using SocialMediaApi.Domain.Events.Groups;
using SocialMediaApi.Domain.Exceptions;
using SocialMediaApi.Domain.Mappers;
using SocialMediaApi.Domain.Models.Groups;
using SocialMediaApi.Domain.ViewModels;
using SocialMediaApi.Domain.Interfaces;
using SocialMediaApi.Domain.Logic.EventHandlers;

namespace SocialMediaApi.Domain.Logic.Services
{
    public class GroupService : IGroupService
    {
        private readonly SocialMediaApiDbContext _dbContext;
        private readonly IAuthService _authService;
        private readonly EventHandlerContainer _publisher;

        public GroupService(SocialMediaApiDbContext dbContext, IAuthService authService, EventHandlerContainer publisher)
        {
            _dbContext = dbContext;
            _authService = authService;
            _publisher = publisher;
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
                Description = model.Description ?? string.Empty,
                Id = Guid.NewGuid(),
                EntityStatus = EntityStatus.Ready,
                LastModifiedDate = DateTimeOffset.UtcNow,
                Creator = authUser,
            };
            var addedEntity = await _dbContext.AddAsync(entity);
            await _dbContext.SaveChangesAsync();
            await _publisher.PublishAsync(new AddGroupEvent { Group = addedEntity.Entity });
            return GroupMapper.ToView(addedEntity.Entity)!;
        }

        public async Task DeleteGroupAsync(Guid id)
        {
            var group = await _dbContext.Groups.FindAsync(id) ?? throw new SocialMediaException("No Group found for given Id.");
            var authUser = await _authService.GetAuthorizedUser();
            if (!authUser.Id.Equals(group.Creator.Id))
            {
                throw new SocialMediaException("Group can only be deleted by the creator.");
            }
            group.EntityStatus = EntityStatus.Deleted;
            _dbContext.Groups.Update(group);
            await _dbContext.SaveChangesAsync();
            await _publisher.PublishAsync(new DeleteGroupEvent { Group = group });
        }

        public async Task<GroupViewModel?> GetGroupAsync(Guid id)
        {
            return GroupMapper.ToView(await _dbContext.Groups.FindAsync(id));
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
            var authUser = await _authService.GetAuthorizedUser();
            if (!authUser.Id.Equals(group.Creator.Id))
            {
                throw new SocialMediaException("Group can only be updated by the creator.");
            }
            group.Name = model.Name;
            group.Description = model.Description;
            group.LastModifiedDate = DateTimeOffset.UtcNow;
            _dbContext.Update(group);
            await _dbContext.SaveChangesAsync();
            await _publisher.PublishAsync(new UpdateGroupEvent { Group = group });
            return GroupMapper.ToView(group)!;
        }
    }
}