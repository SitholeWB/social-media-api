using Microsoft.EntityFrameworkCore;
using Pagination.EntityFrameworkCore.Extensions;
using SocialMediaApi.Data;
using SocialMediaApi.Domain.Entities;
using SocialMediaApi.Domain.Entities.Base;
using SocialMediaApi.Domain.Enums;
using SocialMediaApi.Domain.Exceptions;
using SocialMediaApi.Domain.Mappers;
using SocialMediaApi.Domain.Models.ActivePosts;
using SocialMediaApi.Domain.ViewModels;
using SocialMediaApi.Interfaces;
using SocialMediaApi.Logic.Helpers;

namespace SocialMediaApi.Logic.Services
{
    public class ActivePostService : IActivePostService
    {
        private readonly SocialMediaApiDbContext _dbContext;
        private readonly IConfigService _configService;
        private readonly IUserDetailsService _userDetailsService;
        private readonly IAuthService _authService;

        public ActivePostService(SocialMediaApiDbContext dbContext, IConfigService configService, IUserDetailsService userDetailsService, IAuthService authService)
        {
            _dbContext = dbContext;
            _configService = configService;
            _userDetailsService = userDetailsService;
            _authService = authService;
        }

        public async Task AddActivePostAsync(Guid ownerId, AddActivePostModel model)
        {
            if (model?.Post == null)
            {
                throw new SocialMediaException("Post is required.");
            }

            var entity = BasePost.GetChild<ActivePost>(model.Post);
            entity.OwnerId = ownerId;
            await _dbContext.AddAsync(entity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteActivePostAsync(Guid ownerId, Guid id)
        {
            var post = await _dbContext.ActivePosts.FindAsync(id);
            if (post != null)
            {
                _dbContext.ActivePosts.Remove(post);
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task DeleteExpiredActivePostAsync()
        {
            var limit = 1000;
            var hasNext = true;
            do
            {
                var posts = await _dbContext.ActivePosts.Where(x => x.ActionBasedDate <= DateTimeOffset.UtcNow).Take(limit).ToListAsync();

                _dbContext.ActivePosts.RemoveRange(posts);
                await _dbContext.SaveChangesAsync();
                hasNext = posts.Count > limit;
            } while (hasNext);
        }

        public async Task<PostViewModel?> GetActivePostAsync(Guid ownerId, Guid id)
        {
            var reactions = await UserDetailsReactionHelper.GetPostReactionsAsync(_authService, _userDetailsService);
            return PostMapper.ToView(await _dbContext.ActivePosts.FindAsync(id), reactions);
        }

        public async Task<Pagination<PostViewModel>> GetActivePostsAsync(Guid ownerId, int page = 1, int limit = 20)
        {
            var reactions = await UserDetailsReactionHelper.GetPostReactionsAsync(_authService, _userDetailsService);
            return await _dbContext.AsPaginationAsync<ActivePost, PostViewModel>(page, limit, x => x.OwnerId == ownerId, p => PostMapper.ToView(p, reactions)!, sortColumn: nameof(ActivePost.ActionBasedDate), orderByDescending: true);
        }

        public async Task UpdateActivePostAsync(Guid ownerId, Guid id, UpdateActivePostModel model)
        {
            if (model?.Post == null)
            {
                throw new SocialMediaException("Post is required.");
            }
            var post = await _dbContext.ActivePosts.FindAsync(id) ?? throw new SocialMediaException("No Post found for given Id & groupId.");
            if (!post.OwnerId.Equals(ownerId))
            {
                throw new SocialMediaException("No Post found for given Id & groupId.");
            }

            post.Text = model!.Post.Text;
            post.Media = model!.Post.Media;
            post.LastModifiedDate = model!.Post.LastModifiedDate;
            _dbContext.Update(post);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateActiveCommentCountAsync(Guid ownerId, Guid id, bool increment)
        {
            var post = await _dbContext.ActivePosts.FindAsync(id) ?? throw new SocialMediaException("No Post found for given Id & groupId.");
            if (!post.OwnerId.Equals(ownerId))
            {
                throw new SocialMediaException("No Post found for given Id & groupId.");
            }
            if (increment)
            {
                post.TotalComments += 1;
            }
            else
            {
                post.TotalComments -= 1;
            }
            _dbContext.Update(post);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateActivePostExpireDateAsync(Guid ownerId, Guid id, EntityActionType entityActionType)
        {
            var post = await _dbContext.ActivePosts.FindAsync(id) ?? throw new SocialMediaException("No Post found for given Id & groupId.");
            if (!post.OwnerId.Equals(ownerId))
            {
                throw new SocialMediaException("No Post found for given Id & groupId.");
            }
            var entityActionConfig = await _configService.GetActionConfigAsync(entityActionType);
            post.ActionBasedDate = post.ActionBasedDate.AddMinutes(entityActionConfig.ExpireDateMinutes);
            _dbContext.Update(post);
            await _dbContext.SaveChangesAsync();
        }
    }
}