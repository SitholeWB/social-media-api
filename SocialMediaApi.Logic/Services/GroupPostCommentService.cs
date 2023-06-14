using Pagination.EntityFrameworkCore.Extensions;
using SocialMediaApi.Data;
using SocialMediaApi.Domain.Entities;
using SocialMediaApi.Domain.Entities.JsonEntities;
using SocialMediaApi.Domain.Enums;
using SocialMediaApi.Domain.Events.GroupPostComments;
using SocialMediaApi.Domain.Exceptions;
using SocialMediaApi.Domain.Mappers;
using SocialMediaApi.Domain.Models.GroupPostComments;
using SocialMediaApi.Domain.ViewModels;
using SocialMediaApi.Interfaces;
using SocialMediaApi.Logic.EventHandlers;

namespace SocialMediaApi.Logic.Services
{
    public class GroupPostCommentService : IGroupPostCommentService
    {
        private readonly SocialMediaApiDbContext _dbContext;
        private readonly IAuthService _authService;
        private readonly EventHandlerContainer _publisher;
        private readonly IConfigService _configService;

        public GroupPostCommentService(SocialMediaApiDbContext dbContext, IAuthService authService, EventHandlerContainer publisher, IConfigService configService)
        {
            _dbContext = dbContext;
            _authService = authService;
            _publisher = publisher;
            _configService = configService;
        }

        public async Task<GroupPostCommentViewModel> AddGroupPostCommentAsync(Guid groupPostId, AddGroupPostCommentModel model)
        {
            if (string.IsNullOrEmpty(model?.Text))
            {
                throw new SocialMediaException("Text is required.");
            }
            if (model?.Media == null)
            {
                throw new SocialMediaException("Media explicit definition is required.");
            }
            var groupPost = await _dbContext.GroupPosts.FindAsync(groupPostId) ?? throw new SocialMediaException("No Post found for given Id.");
            var authUser = await _authService.GetAuthorizedUser();
            var entityGroupPostConfig = await _configService.GetEntityGroupPostConfigAsync();
            var entity = new GroupPostComment
            {
                CreatedDate = DateTimeOffset.UtcNow,
                Id = Guid.NewGuid(),
                EntityStatus = EntityStatus.Ready,
                LastModifiedDate = DateTimeOffset.UtcNow,
                ActionBasedDate = DateTimeOffset.UtcNow.AddMinutes(entityGroupPostConfig.GroupPostDefaultExpireMinutes),
                Creator = authUser,
                Text = model!.Text,
                Downloads = 0,
                GroupPostId = groupPostId,
                Reactions = new Reaction
                {
                    Emojis = new List<Emoji>(),
                    ReactionsCount = 0
                },
                TotalComments = 0,
                Rank = 0,
                Views = 0,
                GroupPost = groupPost,
                Media = model!.Media
            };
            var addedEntity = await _dbContext.AddAsync(entity);
            groupPost.TotalComments += 1;
            _dbContext.Update(groupPost);
            await _dbContext.SaveChangesAsync();
            await _publisher.PublishAsync(new AddGroupPostCommentEvent
            {
                GroupPostComment = addedEntity.Entity,
                GroupPost = groupPost,
            });
            return GroupPostMapper.ToView(addedEntity.Entity)!;
        }

        public async Task DeleteGroupPostCommentAsync(Guid groupPostId, Guid id)
        {
            var groupPostComment = await _dbContext.GroupPostComments.FindAsync(id) ?? throw new SocialMediaException("No Post Comment found for given Id & groupId.");
            if (!groupPostComment.GroupPostId.Equals(groupPostId))
            {
                throw new SocialMediaException("No Post Comment found for given Id & groupPostId.");
            }
            var groupPost = await _dbContext.GroupPosts.FindAsync(groupPostId) ?? throw new SocialMediaException("No Post found for given Id.");

            var authUser = await _authService.GetAuthorizedUser();
            if (!authUser.Id.Equals(groupPostComment.Creator.Id))
            {
                throw new SocialMediaException("Post Comment can only be deleted by the creator.");
            }
            groupPostComment.EntityStatus = EntityStatus.Deleted;
            _dbContext.GroupPostComments.Update(groupPostComment);
            groupPost.TotalComments -= 1;
            _dbContext.Update(groupPost);
            await _dbContext.SaveChangesAsync();
            await _publisher.PublishAsync(new DeleteGroupPostCommentEvent
            {
                GroupPost = groupPost,
                GroupPostComment = groupPostComment
            });
        }

        public async Task<Pagination<GroupPostCommentViewModel>> GetGroupPostCommentsAsync(Guid groupPostId, int page = 1, int limit = 20)
        {
            return await _dbContext.AsPaginationAsync<GroupPostComment, GroupPostCommentViewModel>(page, limit, x => x.GroupPostId == groupPostId, GroupPostMapper.ToView!, sortColumn: nameof(GroupPostComment.ActionBasedDate), orderByDescending: true);
        }

        public async Task<GroupPostCommentViewModel> UpdateGroupPostCommentAsync(Guid groupPostId, Guid id, UpdateGroupPostCommentModel model)
        {
            if (string.IsNullOrEmpty(model?.Text))
            {
                throw new SocialMediaException("Text is required.");
            }
            var groupPostComment = await _dbContext.GroupPostComments.FindAsync(id) ?? throw new SocialMediaException("No Post Comment found for given Id & groupId.");
            if (!groupPostComment.GroupPostId.Equals(groupPostId))
            {
                throw new SocialMediaException("No Post Comment found for given Id & groupId.");
            }
            var authUser = await _authService.GetAuthorizedUser();
            if (!authUser.Id.Equals(groupPostComment.Creator.Id))
            {
                throw new SocialMediaException("Post Comment can only be updated by the creator.");
            }
            var groupPost = await _dbContext.GroupPosts.FindAsync(groupPostId) ?? throw new SocialMediaException("No Post found for given Id.");

            groupPostComment.Text = model.Text;
            groupPostComment.Media = model.Media;
            groupPostComment.LastModifiedDate = DateTimeOffset.UtcNow;
            _dbContext.Update(groupPostComment);
            await _dbContext.SaveChangesAsync();
            await _publisher.PublishAsync(new UpdateGroupPostCommentEvent
            {
                GroupPost = groupPost,
                GroupPostComment = groupPostComment
            });
            return GroupPostMapper.ToView(groupPostComment)!;
        }

        public async Task UpdateGroupPostCommentExpireDateAsync(Guid groupPostId, Guid id, EntityActionType entityActionType)
        {
            var groupPostComment = await _dbContext.GroupPostComments.FindAsync(id) ?? throw new SocialMediaException("No Post Comment found for given Id & groupId.");
            if (!groupPostComment.GroupPostId.Equals(groupPostId))
            {
                throw new SocialMediaException("No Post Comment found for given Id & groupId.");
            }
            var entityActionConfig = await _configService.GetActionConfigAsync(entityActionType);
            groupPostComment.ActionBasedDate = groupPostComment.ActionBasedDate.AddMinutes(entityActionConfig.ExpireDateMinutes);
            groupPostComment.Rank += entityActionConfig.RankIncrement;
            _dbContext.Update(groupPostComment);
            await _dbContext.SaveChangesAsync();
        }
    }
}