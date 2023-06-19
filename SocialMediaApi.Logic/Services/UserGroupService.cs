﻿using Microsoft.EntityFrameworkCore;
using SocialMediaApi.Data;
using SocialMediaApi.Domain.Entities;
using SocialMediaApi.Domain.Entities.JsonEntities;
using SocialMediaApi.Domain.Models.UserGroups;
using SocialMediaApi.Interfaces;

namespace SocialMediaApi.Logic.Services
{
    public class UserGroupService : IUserGroupService
    {
        private readonly SocialMediaApiDbContext _dbContext;
        private readonly IAuthService _authService;

        public UserGroupService(SocialMediaApiDbContext dbContext, IAuthService authService)
        {
            _dbContext = dbContext;
            _authService = authService;
        }

        public async Task AddUserGroupAsync(AddUserGroupModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }
            var authUser = await _authService.GetAuthorizedUser();
            var userDetails = await _dbContext.UserDetails.FindAsync(authUser.Id);
            if (userDetails == null)
            {
                userDetails = new UserDetails
                {
                    Id = authUser.Id,
                    CreatedDate = DateTimeOffset.UtcNow,
                    LastModifiedDate = DateTimeOffset.UtcNow,
                    CommentReactions = new List<MiniReaction>(),
                    PostReactions = new List<MiniReaction>(),
                    Groups = new List<MiniEntity>()
                };
                userDetails.Groups.Add(new MiniEntity
                {
                    EntityId = model.EntityId,
                    CreatedDate = model.CreatedDate,
                });
                await _dbContext.SaveChangesAsync();
            }
            else
            {
                var group = userDetails.Groups.FirstOrDefault(x => x.EntityId == model.EntityId);
                userDetails.Groups = userDetails.Groups.Where(x => x.EntityId != model.EntityId).ToList();
                if (group == null)
                {
                    group = new MiniEntity
                    {
                        EntityId = model.EntityId,
                        CreatedDate = model.CreatedDate,
                    };
                }
                userDetails.Groups.Add(group);
                _dbContext.Update(userDetails);
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task DeleteUserGroupAsync(Guid entityId)
        {
            var authUser = await _authService.GetAuthorizedUser();
            var userDetails = await _dbContext.UserDetails.FindAsync(authUser.Id);
            if (userDetails != null)
            {
                userDetails.Groups = userDetails.Groups.Where(x => x.EntityId != entityId).ToList();
                _dbContext.Update(userDetails);
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task<IList<MiniEntity>> GetUserGroupsAsync()
        {
            var authUser = await _authService.GetAuthorizedUser();
            return await _dbContext.UserDetails.AsNoTracking().Where(x => x.Id == authUser.Id).Select(x => x.Groups).FirstOrDefaultAsync() ?? new List<MiniEntity>();
        }
    }
}