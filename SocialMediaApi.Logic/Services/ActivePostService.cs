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

namespace SocialMediaApi.Logic.Services
{
    public class ActivePostService : IActivePostService
    {
        private readonly SocialMediaApiDbContext _dbContext;
        private readonly IConfigService _configService;

        public ActivePostService(SocialMediaApiDbContext dbContext, IConfigService configService)
        {
            _dbContext = dbContext;
            _configService = configService;
        }

        public async Task AddActivePostAsync(Guid groupId, AddActivePostModel model)
        {
            if (model?.Post == null)
            {
                throw new SocialMediaException("Post is required.");
            }

            var entity = BasePost.GetChild<ActivePost>(model.Post);
            entity.GroupId = groupId;
            await _dbContext.AddAsync(entity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteActivePostAsync(Guid groupId, Guid id)
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

        public async Task<PostViewModel?> GetActivePostAsync(Guid groupId, Guid id)
        {
            return PostMapper.ToView(await _dbContext.ActivePosts.FindAsync(id));
        }

        public async Task<Pagination<PostViewModel>> GetActivePostsAsync(Guid groupId, int page = 1, int limit = 20)
        {
            return await _dbContext.AsPaginationAsync<ActivePost, PostViewModel>(page, limit, x => x.GroupId == groupId, PostMapper.ToView!, sortColumn: nameof(ActivePost.ActionBasedDate), orderByDescending: true);
        }

        public async Task UpdateActivePostAsync(Guid groupId, Guid id, UpdateActivePostModel model)
        {
            if (model?.Post == null)
            {
                throw new SocialMediaException("Post is required.");
            }
            var post = await _dbContext.ActivePosts.FindAsync(id) ?? throw new SocialMediaException("No Post found for given Id & groupId.");
            if (!post.GroupId.Equals(groupId))
            {
                throw new SocialMediaException("No Post found for given Id & groupId.");
            }

            post.Text = model!.Post.Text;
            post.Media = model!.Post.Media;
            post.LastModifiedDate = model!.Post.LastModifiedDate;
            _dbContext.Update(post);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateActiveCommentCountAsync(Guid groupId, Guid id, bool increment)
        {
            var post = await _dbContext.ActivePosts.FindAsync(id) ?? throw new SocialMediaException("No Post found for given Id & groupId.");
            if (!post.GroupId.Equals(groupId))
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

        public async Task UpdateActivePostExpireDateAsync(Guid groupId, Guid id, EntityActionType entityActionType)
        {
            var post = await _dbContext.ActivePosts.FindAsync(id) ?? throw new SocialMediaException("No Post found for given Id & groupId.");
            if (!post.GroupId.Equals(groupId))
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