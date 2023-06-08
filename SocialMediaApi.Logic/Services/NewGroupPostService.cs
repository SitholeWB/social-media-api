using Pagination.EntityFrameworkCore.Extensions;
using SocialMediaApi.Data;
using SocialMediaApi.Domain.Entities;
using SocialMediaApi.Domain.Entities.Base;
using SocialMediaApi.Domain.Enums;
using SocialMediaApi.Domain.Exceptions;
using SocialMediaApi.Domain.Mappers;
using SocialMediaApi.Domain.Models.NewGroupPosts;
using SocialMediaApi.Domain.ViewModels;
using SocialMediaApi.Interfaces;

namespace SocialMediaApi.Logic.Services
{
    public class NewGroupPostService : INewGroupPostService
    {
        private readonly SocialMediaApiDbContext _dbContext;
        private readonly IConfigService _configService;

        public NewGroupPostService(SocialMediaApiDbContext dbContext, IConfigService configService)
        {
            _dbContext = dbContext;
            _configService = configService;
        }

        public async Task AddNewGroupPostAsync(Guid groupId, AddNewGroupPostModel model)
        {
            if (model?.GroupPost == null)
            {
                throw new SocialMediaException("GroupPost is required.");
            }

            var entity = BasePost.GetChild<NewGroupPost>(model.GroupPost);
            entity.GroupId = groupId;
            await _dbContext.AddAsync(entity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteNewGroupPostAsync(Guid groupId, Guid id)
        {
            var groupPost = await _dbContext.NewGroupPosts.FindAsync(id) ?? throw new SocialMediaException("No Post found for given Id & groupId.");
            if (!groupPost.GroupId.Equals(groupId))
            {
                throw new SocialMediaException("No Post found for given Id & groupId.");
            }
            _dbContext.NewGroupPosts.Remove(groupPost);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<GroupPostViewModel?> GetNewGroupPostAsync(Guid groupId, Guid id)
        {
            return GroupPostMapper.ToView(await _dbContext.NewGroupPosts.FindAsync(id));
        }

        public async Task<Pagination<GroupPostViewModel>> GetNewGroupPostsAsync(Guid groupId, int page = 1, int limit = 20)
        {
            return await _dbContext.AsPaginationAsync<NewGroupPost, GroupPostViewModel>(page, limit, x => x.GroupId == groupId, GroupPostMapper.ToView!);
        }

        public async Task UpdateNewGroupPostAsync(Guid groupId, Guid id, UpdateNewGroupPostModel model)
        {
            if (model?.GroupPost == null)
            {
                throw new SocialMediaException("GroupPost is required.");
            }
            var groupPost = await _dbContext.NewGroupPosts.FindAsync(id) ?? throw new SocialMediaException("No Post found for given Id & groupId.");
            if (!groupPost.GroupId.Equals(groupId))
            {
                throw new SocialMediaException("No Post found for given Id & groupId.");
            }

            groupPost.Text = model!.GroupPost.Text;
            groupPost.ThumbnailUrl = model!.GroupPost.ThumbnailUrl;
            groupPost.Media = model!.GroupPost.Media;
            groupPost.LastModifiedDate = model!.GroupPost.LastModifiedDate;
            groupPost.Rank = model!.GroupPost.Rank;
            _dbContext.Update(groupPost);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateNewGroupPostRankAsync(Guid groupId, Guid id, EntityActionType entityActionType)
        {
            var groupPost = await _dbContext.NewGroupPosts.FindAsync(id) ?? throw new SocialMediaException("No Post found for given Id & groupId.");
            var rank = await _configService.GetRankingConfig(entityActionType);
            groupPost.Rank += rank;
            _dbContext.Update(groupPost);
            await _dbContext.SaveChangesAsync();
        }
    }
}