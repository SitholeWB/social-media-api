using Pagination.EntityFrameworkCore.Extensions;
using SocialMediaApi.Data;
using SocialMediaApi.Domain.Entities;
using SocialMediaApi.Domain.Entities.JsonEntities;
using SocialMediaApi.Domain.Enums;
using SocialMediaApi.Domain.Events.PostComments;
using SocialMediaApi.Domain.Exceptions;
using SocialMediaApi.Domain.Mappers;
using SocialMediaApi.Domain.Models.PostComments;
using SocialMediaApi.Domain.ViewModels;
using SocialMediaApi.Interfaces;
using SocialMediaApi.Logic.EventHandlers;

namespace SocialMediaApi.Logic.Services
{
    public class PostCommentService : IPostCommentService
    {
        private readonly SocialMediaApiDbContext _dbContext;
        private readonly IAuthService _authService;
        private readonly EventHandlerContainer _publisher;
        private readonly IConfigService _configService;

        public PostCommentService(SocialMediaApiDbContext dbContext, IAuthService authService, EventHandlerContainer publisher, IConfigService configService)
        {
            _dbContext = dbContext;
            _authService = authService;
            _publisher = publisher;
            _configService = configService;
        }

        public async Task<PostCommentViewModel> AddPostCommentAsync(Guid postId, AddPostCommentModel model)
        {
            if (string.IsNullOrEmpty(model?.Text))
            {
                throw new SocialMediaException("Text is required.");
            }
            if (model?.Media == null)
            {
                throw new SocialMediaException("Media explicit definition is required.");
            }
            var post = await _dbContext.Posts.FindAsync(postId) ?? throw new SocialMediaException("No Post found for given Id.");
            var authUser = await _authService.GetAuthorizedUser();
            var entityPostConfig = await _configService.GetEntityPostConfigAsync();
            var entity = new PostComment
            {
                CreatedDate = DateTimeOffset.UtcNow,
                Id = Guid.NewGuid(),
                EntityStatus = EntityStatus.Ready,
                LastModifiedDate = DateTimeOffset.UtcNow,
                ActionBasedDate = DateTimeOffset.UtcNow.AddMinutes(entityPostConfig.PostDefaultExpireMinutes),
                Creator = authUser,
                Text = model!.Text,
                Downloads = 0,
                PostId = postId,
                Reactions = new ReactionSummary
                {
                    Emojis = new List<Emoji>(),
                    ReactionsCount = 0
                },
                TotalComments = 0,
                Rank = 0,
                Views = 0,
                Post = post,
                Media = model!.Media
            };
            var addedEntity = await _dbContext.AddAsync(entity);
            post.TotalComments += 1;
            _dbContext.Update(post);
            await _dbContext.SaveChangesAsync();
            await _publisher.PublishAsync(new AddPostCommentEvent
            {
                PostComment = addedEntity.Entity,
                Post = post,
            });
            return PostMapper.ToView(addedEntity.Entity)!;
        }

        public async Task DeletePostCommentAsync(Guid postId, Guid id)
        {
            var postComment = await _dbContext.PostComments.FindAsync(id) ?? throw new SocialMediaException("No Post Comment found for given Id & groupId.");
            if (!postComment.PostId.Equals(postId))
            {
                throw new SocialMediaException("No Post Comment found for given Id & postId.");
            }
            var post = await _dbContext.Posts.FindAsync(postId) ?? throw new SocialMediaException("No Post found for given Id.");

            var authUser = await _authService.GetAuthorizedUser();
            if (!authUser.Id.Equals(postComment.Creator.Id))
            {
                throw new SocialMediaException("Post Comment can only be deleted by the creator.");
            }
            postComment.EntityStatus = EntityStatus.Deleted;
            _dbContext.PostComments.Update(postComment);
            post.TotalComments -= 1;
            _dbContext.Update(post);
            await _dbContext.SaveChangesAsync();
            await _publisher.PublishAsync(new DeletePostCommentEvent
            {
                Post = post,
                PostComment = postComment
            });
        }

        public async Task<Pagination<PostCommentViewModel>> GetPostCommentsAsync(Guid postId, int page = 1, int limit = 20)
        {
            return await _dbContext.AsPaginationAsync<PostComment, PostCommentViewModel>(page, limit, x => x.PostId == postId, PostMapper.ToView!, sortColumn: nameof(PostComment.ActionBasedDate), orderByDescending: true);
        }

        public async Task<PostCommentViewModel> UpdatePostCommentAsync(Guid postId, Guid id, UpdatePostCommentModel model)
        {
            if (string.IsNullOrEmpty(model?.Text))
            {
                throw new SocialMediaException("Text is required.");
            }
            var postComment = await _dbContext.PostComments.FindAsync(id) ?? throw new SocialMediaException("No Post Comment found for given Id & groupId.");
            if (!postComment.PostId.Equals(postId))
            {
                throw new SocialMediaException("No Post Comment found for given Id & groupId.");
            }
            var authUser = await _authService.GetAuthorizedUser();
            if (!authUser.Id.Equals(postComment.Creator.Id))
            {
                throw new SocialMediaException("Post Comment can only be updated by the creator.");
            }
            var post = await _dbContext.Posts.FindAsync(postId) ?? throw new SocialMediaException("No Post found for given Id.");

            postComment.Text = model.Text;
            postComment.Media = model.Media;
            postComment.LastModifiedDate = DateTimeOffset.UtcNow;
            _dbContext.Update(postComment);
            await _dbContext.SaveChangesAsync();
            await _publisher.PublishAsync(new UpdatePostCommentEvent
            {
                Post = post,
                PostComment = postComment
            });
            return PostMapper.ToView(postComment)!;
        }

        public async Task UpdatePostCommentExpireDateAsync(Guid postId, Guid id, EntityActionType entityActionType)
        {
            var postComment = await _dbContext.PostComments.FindAsync(id) ?? throw new SocialMediaException("No Post Comment found for given Id & groupId.");
            if (!postComment.PostId.Equals(postId))
            {
                throw new SocialMediaException("No Post Comment found for given Id & groupId.");
            }
            var entityActionConfig = await _configService.GetActionConfigAsync(entityActionType);
            postComment.ActionBasedDate = postComment.ActionBasedDate.AddMinutes(entityActionConfig.ExpireDateMinutes);
            postComment.Rank += entityActionConfig.RankIncrement;
            _dbContext.Update(postComment);
            await _dbContext.SaveChangesAsync();
        }
    }
}