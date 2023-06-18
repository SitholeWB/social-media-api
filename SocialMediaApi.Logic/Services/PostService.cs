using Pagination.EntityFrameworkCore.Extensions;
using SocialMediaApi.Data;
using SocialMediaApi.Domain.Entities;
using SocialMediaApi.Domain.Entities.JsonEntities;
using SocialMediaApi.Domain.Enums;
using SocialMediaApi.Domain.Events.Posts;
using SocialMediaApi.Domain.Exceptions;
using SocialMediaApi.Domain.Mappers;
using SocialMediaApi.Domain.Models.Posts;
using SocialMediaApi.Domain.ViewModels;
using SocialMediaApi.Interfaces;
using SocialMediaApi.Logic.EventHandlers;

namespace SocialMediaApi.Logic.Services
{
    public class PostService : IPostService
    {
        private readonly SocialMediaApiDbContext _dbContext;
        private readonly IAuthService _authService;
        private readonly EventHandlerContainer _publisher;
        private readonly IConfigService _configService;
        private readonly IUserDetailsService _userDetailsService;

        public PostService(SocialMediaApiDbContext dbContext, IAuthService authService, EventHandlerContainer publisher, IConfigService configService, IUserDetailsService userDetailsService)
        {
            _dbContext = dbContext;
            _authService = authService;
            _publisher = publisher;
            _configService = configService;
            _userDetailsService = userDetailsService;
        }

        public async Task<PostViewModel> AddPostAsync(Guid ownerId, AddPostModel model)
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
            var entity = new Post
            {
                CreatedDate = DateTimeOffset.UtcNow,
                Id = Guid.NewGuid(),
                EntityStatus = EntityStatus.Ready,
                LastModifiedDate = DateTimeOffset.UtcNow,
                ActionBasedDate = DateTimeOffset.UtcNow.AddMinutes(entityPostConfig.PostDefaultExpireMinutes),
                Creator = authUser,
                Text = model!.Text,
                Downloads = 0,
                OwnerId = ownerId,
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
            await _dbContext.SaveChangesAsync();

            await _publisher.PublishAsync(new AddPostEvent { Post = addedEntity.Entity });

            return PostMapper.ToView(addedEntity.Entity, new List<MiniReaction> { })!;
        }

        public async Task DeletePostAsync(Guid ownerId, Guid id)
        {
            var post = await _dbContext.Posts.FindAsync(id) ?? throw new SocialMediaException("No Post found for given Id & groupId.");
            if (!post.OwnerId.Equals(ownerId))
            {
                throw new SocialMediaException("No Post found for given Id & groupId.");
            }
            var authUser = await _authService.GetAuthorizedUser();
            if (!authUser.Id.Equals(post.Creator.Id))
            {
                throw new SocialMediaException("Post can only be deleted by the creator.");
            }
            post.EntityStatus = EntityStatus.Deleted;
            _dbContext.Posts.Update(post);
            await _dbContext.SaveChangesAsync();

            await _publisher.PublishAsync(new DeletePostEvent { Post = post });
        }

        public async Task<PostViewModel?> GetPostAsync(Guid ownerId, Guid id)
        {
            var reactions = await GetPostReactionsAsync();
            return PostMapper.ToView(await _dbContext.Posts.FindAsync(id), reactions);
        }

        public async Task<Pagination<PostViewModel>> GetPostsAsync(Guid ownerId, int page = 1, int limit = 20)
        {
            var reactions = await GetPostReactionsAsync();
            return await _dbContext.AsPaginationAsync<Post, PostViewModel>(page, limit, x => x.OwnerId == ownerId, p => PostMapper.ToView(p, reactions)!, sortColumn: nameof(Post.ActionBasedDate), orderByDescending: true);
        }

        public async Task UpdatePostExpireDateAsync(Guid ownerId, Guid id, EntityActionType entityActionType)
        {
            var post = await _dbContext.Posts.FindAsync(id) ?? throw new SocialMediaException("No Post found for given Id & groupId.");
            if (!post.OwnerId.Equals(ownerId))
            {
                throw new SocialMediaException("No Post found for given Id & groupId.");
            }
            var entityActionConfig = await _configService.GetActionConfigAsync(entityActionType);
            post.ActionBasedDate = post.ActionBasedDate.AddMinutes(entityActionConfig.ExpireDateMinutes);
            post.Rank += entityActionConfig.RankIncrement;
            _dbContext.Update(post);
            await _dbContext.SaveChangesAsync();
        }

        public async Task<PostViewModel> UpdatePostAsync(Guid ownerId, Guid id, UpdatePostModel model)
        {
            if (string.IsNullOrEmpty(model?.Text))
            {
                throw new SocialMediaException("Text is required.");
            }
            var post = await _dbContext.Posts.FindAsync(id) ?? throw new SocialMediaException("No Post found for given Id & groupId.");
            if (!post.OwnerId.Equals(ownerId))
            {
                throw new SocialMediaException("No Post found for given Id & groupId.");
            }
            var authUser = await _authService.GetAuthorizedUser();
            if (!authUser.Id.Equals(post.Creator.Id))
            {
                throw new SocialMediaException("Post can only be updated by the creator.");
            }
            post.Text = model.Text;
            post.Media = model.Media;
            post.LastModifiedDate = DateTimeOffset.UtcNow;
            _dbContext.Update(post);
            await _dbContext.SaveChangesAsync();
            await _publisher.PublishAsync(new UpdatePostEvent { Post = post });
            var reactions = await GetPostReactionsAsync();
            return PostMapper.ToView(post, reactions)!;
        }

        private async Task<IList<MiniReaction>> GetPostReactionsAsync()
        {
            var isAuthenticated = await _authService.IsAuthenticated();
            var reactions = default(IList<MiniReaction>);
            if (isAuthenticated)
            {
                reactions = await _userDetailsService.GetPostReactionsAsync();
            }
            else
            {
                reactions = new List<MiniReaction>();
            }

            return reactions ?? new List<MiniReaction>();
        }
    }
}