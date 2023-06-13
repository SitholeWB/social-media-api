﻿using Microsoft.EntityFrameworkCore;
using Pagination.EntityFrameworkCore.Extensions;
using SocialMediaApi.Data;
using SocialMediaApi.Domain.Entities;
using SocialMediaApi.Domain.Entities.Base;
using SocialMediaApi.Domain.Enums;
using SocialMediaApi.Domain.Exceptions;
using SocialMediaApi.Domain.Mappers;
using SocialMediaApi.Domain.Models.ActiveGroupPosts;
using SocialMediaApi.Domain.ViewModels;
using SocialMediaApi.Interfaces;

namespace SocialMediaApi.Logic.Services
{
    public class ActiveGroupPostService : IActiveGroupPostService
    {
        private readonly SocialMediaApiDbContext _dbContext;
        private readonly IConfigService _configService;

        public ActiveGroupPostService(SocialMediaApiDbContext dbContext, IConfigService configService)
        {
            _dbContext = dbContext;
            _configService = configService;
        }

        public async Task AddActiveGroupPostAsync(Guid groupId, AddActiveGroupPostModel model)
        {
            if (model?.GroupPost == null)
            {
                throw new SocialMediaException("GroupPost is required.");
            }

            var entity = BasePost.GetChild<ActiveGroupPost>(model.GroupPost);
            entity.GroupId = groupId;
            await _dbContext.AddAsync(entity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteActiveGroupPostAsync(Guid groupId, Guid id)
        {
            var groupPost = await _dbContext.ActiveGroupPosts.FindAsync(id);
            if (groupPost != null)
            {
                _dbContext.ActiveGroupPosts.Remove(groupPost);
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task DeleteExpiredActiveGroupPostAsync()
        {
            var limit = 1000;
            var hasNext = true;
            do
            {
                var groupPosts = await _dbContext.ActiveGroupPosts.Where(x => x.ActionBasedDate <= DateTimeOffset.UtcNow).Take(limit).ToListAsync();

                _dbContext.ActiveGroupPosts.RemoveRange(groupPosts);
                await _dbContext.SaveChangesAsync();
                hasNext = groupPosts.Count > limit;
            } while (hasNext);
        }

        public async Task<GroupPostViewModel?> GetActiveGroupPostAsync(Guid groupId, Guid id)
        {
            return GroupPostMapper.ToView(await _dbContext.ActiveGroupPosts.FindAsync(id));
        }

        public async Task<Pagination<GroupPostViewModel>> GetActiveGroupPostsAsync(Guid groupId, int page = 1, int limit = 20)
        {
            return await _dbContext.AsPaginationAsync<ActiveGroupPost, GroupPostViewModel>(page, limit, x => x.GroupId == groupId, GroupPostMapper.ToView!, sortColumn: nameof(ActiveGroupPost.ActionBasedDate), orderByDescending: true);
        }

        public async Task UpdateActiveGroupPostAsync(Guid groupId, Guid id, UpdateActiveGroupPostModel model)
        {
            if (model?.GroupPost == null)
            {
                throw new SocialMediaException("GroupPost is required.");
            }
            var groupPost = await _dbContext.ActiveGroupPosts.FindAsync(id) ?? throw new SocialMediaException("No Post found for given Id & groupId.");
            if (!groupPost.GroupId.Equals(groupId))
            {
                throw new SocialMediaException("No Post found for given Id & groupId.");
            }

            groupPost.Text = model!.GroupPost.Text;
            groupPost.Media = model!.GroupPost.Media;
            groupPost.LastModifiedDate = model!.GroupPost.LastModifiedDate;
            _dbContext.Update(groupPost);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateActiveGroupPostExpireDateAsync(Guid groupId, Guid id, EntityActionType entityActionType)
        {
            var groupPost = await _dbContext.ActiveGroupPosts.FindAsync(id) ?? throw new SocialMediaException("No Post found for given Id & groupId.");
            if (!groupPost.GroupId.Equals(groupId))
            {
                throw new SocialMediaException("No Post found for given Id & groupId.");
            }
            var entityActionConfig = await _configService.GetActionConfigAsync(entityActionType);
            groupPost.ActionBasedDate = groupPost.ActionBasedDate.AddMinutes(entityActionConfig.ExpireDateMinutes);
            _dbContext.Update(groupPost);
            await _dbContext.SaveChangesAsync();
        }
    }
}