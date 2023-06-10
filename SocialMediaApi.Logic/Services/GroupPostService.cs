using Pagination.EntityFrameworkCore.Extensions;
using SocialMediaApi.Data;
using SocialMediaApi.Domain.Entities;
using SocialMediaApi.Domain.Entities.JsonEntities;
using SocialMediaApi.Domain.Enums;
using SocialMediaApi.Domain.Events.GroupPosts;
using SocialMediaApi.Domain.Exceptions;
using SocialMediaApi.Domain.Mappers;
using SocialMediaApi.Domain.Models.GroupPosts;
using SocialMediaApi.Domain.ViewModels;
using SocialMediaApi.Interfaces;
using SocialMediaApi.Logic.EventHandlers;

namespace SocialMediaApi.Logic.Services
{
    public class GroupPostService : IGroupPostService
    {
        private readonly SocialMediaApiDbContext _dbContext;
        private readonly IAuthService _authService;
        private readonly EventHandlerContainer _publisher;
        private readonly IConfigService _configService;

        public GroupPostService(SocialMediaApiDbContext dbContext, IAuthService authService, EventHandlerContainer publisher, IConfigService configService)
        {
            _dbContext = dbContext;
            _authService = authService;
            _publisher = publisher;
            _configService = configService;
        }

        public async Task<GroupPostViewModel> AddGroupPostAsync(Guid groupId, AddGroupPostModel model)
        {
            if (string.IsNullOrEmpty(model?.Text))
            {
                throw new SocialMediaException("Text is required.");
            }
            if (model?.Media == null)
            {
                throw new SocialMediaException("Media explicit definition is required.");
            }
            var authUser = await _authService.GetAuthorizedUser();
            var entity = new GroupPost
            {
                CreatedDate = DateTimeOffset.UtcNow,
                Id = Guid.NewGuid(),
                EntityStatus = EntityStatus.Ready,
                LastModifiedDate = DateTimeOffset.UtcNow,
                Creator = authUser,
                Text = model!.Text,
                Downloads = 0,
                GroupId = groupId,
                Reactions = new Reaction
                {
                    Emojis = new List<Emoji>(),
                    ReactionsCount = 0
                },
                ThumbnailUrl = model?.ThumbnailUrl ?? "",
                TotalComments = 0,
                Views = 0,
                Media = model!.Media
            };
            var addedEntity = await _dbContext.AddAsync(entity);
            await _dbContext.SaveChangesAsync();
            await _publisher.PublishAsync(new AddGroupPostEvent { GroupPost = addedEntity.Entity });
            return GroupPostMapper.ToView(addedEntity.Entity)!;
        }

        public async Task DeleteGroupPostAsync(Guid groupId, Guid id)
        {
            var groupPost = await _dbContext.GroupPosts.FindAsync(id) ?? throw new SocialMediaException("No Post found for given Id & groupId.");
            if (!groupPost.GroupId.Equals(groupId))
            {
                throw new SocialMediaException("No Post found for given Id & groupId.");
            }
            var authUser = await _authService.GetAuthorizedUser();
            if (!authUser.Id.Equals(groupPost.Creator.Id))
            {
                throw new SocialMediaException("Post can only be deleted by the creator.");
            }
            groupPost.EntityStatus = EntityStatus.Deleted;
            _dbContext.GroupPosts.Update(groupPost);
            await _dbContext.SaveChangesAsync();
            await _publisher.PublishAsync(new DeleteGroupPostEvent { GroupPost = groupPost });
        }

        public async Task<GroupPostViewModel?> GetGroupPostAsync(Guid groupId, Guid id)
        {
            return GroupPostMapper.ToView(await _dbContext.GroupPosts.FindAsync(id));
        }

        public async Task<Pagination<GroupPostViewModel>> GetGroupPostsAsync(Guid groupId, int page = 1, int limit = 20)
        {
            return await _dbContext.GroupPosts.OrderByDescending(x => x.ActionBasedDate).AsPaginationAsync<GroupPost, GroupPostViewModel>(page, limit, x => x.GroupId == groupId, GroupPostMapper.ToView!);
        }

        public async Task UpdateGroupPostRankAsync(Guid groupId, Guid id, EntityActionType entityActionType)
        {
            var groupPost = await _dbContext.GroupPosts.FindAsync(id) ?? throw new SocialMediaException("No Post found for given Id & groupId.");
            if (!groupPost.GroupId.Equals(groupId))
            {
                throw new SocialMediaException("No Post found for given Id & groupId.");
            }
            var entityActionConfig = await _configService.GetActionConfigAsync(entityActionType);
            groupPost.Rank += entityActionConfig.Rank;
            _dbContext.Update(groupPost);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateGroupPostExpireDateAsync(Guid groupId, Guid id, EntityActionType entityActionType)
        {
            var groupPost = await _dbContext.GroupPosts.FindAsync(id) ?? throw new SocialMediaException("No Post found for given Id & groupId.");
            if (!groupPost.GroupId.Equals(groupId))
            {
                throw new SocialMediaException("No Post found for given Id & groupId.");
            }
            var entityActionConfig = await _configService.GetActionConfigAsync(entityActionType);
            groupPost.ActionBasedDate = groupPost.ActionBasedDate.AddMinutes(entityActionConfig.ExpireDateMinutes);
            _dbContext.Update(groupPost);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<GroupPostViewModel> UpdateGroupPostAsync(Guid groupId, Guid id, UpdateGroupPostModel model)
        {
            if (string.IsNullOrEmpty(model?.Text))
            {
                throw new SocialMediaException("Text is required.");
            }
            var groupPost = await _dbContext.GroupPosts.FindAsync(id) ?? throw new SocialMediaException("No Post found for given Id & groupId.");
            if (!groupPost.GroupId.Equals(groupId))
            {
                throw new SocialMediaException("No Post found for given Id & groupId.");
            }
            var authUser = await _authService.GetAuthorizedUser();
            if (!authUser.Id.Equals(groupPost.Creator.Id))
            {
                throw new SocialMediaException("Post can only be updated by the creator.");
            }
            groupPost.Text = model.Text;
            groupPost.ThumbnailUrl = model.ThumbnailUrl;
            groupPost.Media = model.Media;
            groupPost.LastModifiedDate = DateTimeOffset.UtcNow;
            _dbContext.Update(groupPost);
            await _dbContext.SaveChangesAsync();
            await _publisher.PublishAsync(new UpdateGroupPostEvent { GroupPost = groupPost });
            return GroupPostMapper.ToView(groupPost)!;
        }
    }
}