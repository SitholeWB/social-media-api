using Pagination.EntityFrameworkCore.Extensions;
using SocialMediaApi.Data;
using SocialMediaApi.Domain.Entities;
using SocialMediaApi.Domain.Entities.JsonEntities;
using SocialMediaApi.Domain.Enums;
using SocialMediaApi.Domain.Events.Comments;
using SocialMediaApi.Domain.Exceptions;
using SocialMediaApi.Domain.Mappers;
using SocialMediaApi.Domain.Models.Comments;
using SocialMediaApi.Domain.ViewModels;
using SocialMediaApi.Domain.Interfaces;
using SocialMediaApi.Domain.Logic.EventHandlers;
using SocialMediaApi.Domain.Logic.Helpers;

namespace SocialMediaApi.Domain.Logic.Services
{
    public class CommentService : ICommentService
    {
        private readonly SocialMediaApiDbContext _dbContext;
        private readonly IAuthService _authService;
        private readonly EventHandlerContainer _publisher;
        private readonly IConfigService _configService;
        private readonly IUserDetailsService _userDetailsService;

        public CommentService(SocialMediaApiDbContext dbContext, IAuthService authService, EventHandlerContainer publisher, IConfigService configService, IUserDetailsService userDetailsService)
        {
            _dbContext = dbContext;
            _authService = authService;
            _publisher = publisher;
            _configService = configService;
            _userDetailsService = userDetailsService;
        }

        public async Task<CommentViewModel> AddCommentAsync(Guid postId, AddCommentModel model)
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
            var entityPostConfig = await _configService.GetEntityPostConfigAsync();
            var entity = new Comment
            {
                CreatedDate = DateTimeOffset.UtcNow,
                Id = Guid.NewGuid(),
                EntityStatus = EntityStatus.Ready,
                LastModifiedDate = DateTimeOffset.UtcNow,
                ActionBasedDate = DateTimeOffset.UtcNow.AddMinutes(entityPostConfig.PostDefaultExpireMinutes),
                Creator = authUser,
                Text = model!.Text,
                PostId = postId,
                Reactions = new ReactionSummary
                {
                    Emojis = new List<Emoji>(),
                    ReactionsCount = 0
                },
                TotalComments = 0,
                Rank = 0,
                Views = 0,
                Media = model!.Media
            };
            var addedEntity = await _dbContext.AddAsync(entity);
            var post = await _dbContext.Posts.FindAsync(postId);
            if (post != null)
            {
                post.TotalComments += 1;
                _dbContext.Update(post);
            }
            await _dbContext.SaveChangesAsync();
            await _publisher.PublishAsync(new AddCommentEvent
            {
                Comment = addedEntity.Entity,
                Post = post,
            });
            return PostMapper.ToView(addedEntity.Entity, new List<MiniReaction>())!;
        }

        public async Task DeleteCommentAsync(Guid postId, Guid id)
        {
            var comment = await _dbContext.Comments.FindAsync(id) ?? throw new SocialMediaException("No Post Comment found for given Id & groupId.");
            if (!comment.PostId.Equals(postId))
            {
                throw new SocialMediaException("No Post Comment found for given Id & postId.");
            }
            var post = await _dbContext.Posts.FindAsync(postId) ?? throw new SocialMediaException("No Post found for given Id.");

            var authUser = await _authService.GetAuthorizedUser();
            if (!authUser.Id.Equals(comment.Creator.Id))
            {
                throw new SocialMediaException("Post Comment can only be deleted by the creator.");
            }
            comment.EntityStatus = EntityStatus.Deleted;
            _dbContext.Comments.Update(comment);
            post.TotalComments -= 1;
            _dbContext.Update(post);
            await _dbContext.SaveChangesAsync();
            await _publisher.PublishAsync(new DeleteCommentEvent
            {
                Post = post,
                Comment = comment
            });
        }

        public async Task<Pagination<CommentViewModel>> GetCommentsAsync(Guid postId, int page = 1, int limit = 20)
        {
            var reactions = await UserDetailsReactionHelper.GetCommentReactionsAsync(_authService, _userDetailsService);
            return await _dbContext.AsPaginationAsync<Comment, CommentViewModel>(page, limit, x => x.PostId == postId, p => PostMapper.ToView(p, reactions)!, sortColumn: nameof(Comment.ActionBasedDate), orderByDescending: true);
        }

        public async Task<CommentViewModel> UpdateCommentAsync(Guid postId, Guid id, UpdateCommentModel model)
        {
            if (string.IsNullOrEmpty(model?.Text))
            {
                throw new SocialMediaException("Text is required.");
            }
            var comment = await _dbContext.Comments.FindAsync(id) ?? throw new SocialMediaException("No Post Comment found for given Id & groupId.");
            if (!comment.PostId.Equals(postId))
            {
                throw new SocialMediaException("No Post Comment found for given Id & groupId.");
            }
            var authUser = await _authService.GetAuthorizedUser();
            if (!authUser.Id.Equals(comment.Creator.Id))
            {
                throw new SocialMediaException("Post Comment can only be updated by the creator.");
            }
            var post = await _dbContext.Posts.FindAsync(postId) ?? throw new SocialMediaException("No Post found for given Id.");

            comment.Text = model.Text;
            comment.Media = model.Media;
            comment.LastModifiedDate = DateTimeOffset.UtcNow;
            _dbContext.Update(comment);
            await _dbContext.SaveChangesAsync();
            await _publisher.PublishAsync(new UpdateCommentEvent
            {
                Post = post,
                Comment = comment
            });
            var reactions = await UserDetailsReactionHelper.GetCommentReactionsAsync(_authService, _userDetailsService);
            return PostMapper.ToView(comment, reactions)!;
        }

        public async Task UpdateCommentExpireDateAsync(Guid postId, Guid id, EntityActionType entityActionType)
        {
            var comment = await _dbContext.Comments.FindAsync(id) ?? throw new SocialMediaException("No Post Comment found for given Id & groupId.");
            if (!comment.PostId.Equals(postId))
            {
                throw new SocialMediaException("No Post Comment found for given Id & groupId.");
            }
            var entityActionConfig = await _configService.GetActionConfigAsync(entityActionType);
            comment.ActionBasedDate = comment.ActionBasedDate.AddMinutes(entityActionConfig.ExpireDateMinutes);
            comment.Rank += entityActionConfig.RankIncrement;
            _dbContext.Update(comment);
            await _dbContext.SaveChangesAsync();
        }
    }
}