using Microsoft.EntityFrameworkCore;
using Pagination.EntityFrameworkCore.Extensions;
using SocialMediaApi.Data;
using SocialMediaApi.Domain.Common;
using SocialMediaApi.Domain.Entities;
using SocialMediaApi.Domain.Entities.JsonEntities;
using SocialMediaApi.Domain.Mappers;
using SocialMediaApi.Domain.Models.UserPosts;
using SocialMediaApi.Domain.ViewModels;
using SocialMediaApi.Interfaces;

namespace SocialMediaApi.Logic.Services
{
    public class UserPostService : IUserPostService
    {
        private readonly SocialMediaApiDbContext _dbContext;

        public UserPostService(SocialMediaApiDbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task AddUserPostAsync(AddUserPostModel model)
        {
            if (model == null)
            {
                throw new ArgumentNullException(nameof(model));
            }
            var miniEntity = new MiniEntity
            {
                CreatedDate = model.CreatedDate,
                EntityId = model.EntityId,
            };
            var userPost = await _dbContext.UserPosts.OrderByDescending(x => x.Page).FirstOrDefaultAsync(x => x.UserId == model.UserId);
            if (userPost == null)
            {
                userPost = new UserPost
                {
                    Id = GenerateKeys.GetUserPostId(model.UserId, 1, "POST"),
                    Posts = new List<MiniEntity>(),
                    IsFull = false,
                    Page = 1,
                    CreatedDate = DateTimeOffset.UtcNow,
                    UserId = model.UserId,
                };
                userPost.Posts.Add(miniEntity);
                _dbContext.Add(userPost);
            }
            else if (userPost.IsFull)
            {
                var newUserPost = new UserPost
                {
                    Id = GenerateKeys.GetUserPostId(model.UserId, userPost.Page + 1, "POST"),
                    Posts = new List<MiniEntity>(),
                    IsFull = false,
                    Page = userPost.Page + 1,
                    CreatedDate = DateTimeOffset.UtcNow,
                    UserId = model.UserId,
                };
                userPost.Posts.Add(miniEntity);
                _dbContext.Add(newUserPost);
            }
            else
            {
                userPost.Posts.Add(miniEntity);
                userPost.IsFull = userPost.Posts.Count > 99;
                _dbContext.Update(userPost);
            }
            await _dbContext.SaveChangesAsync();
        }

        public async Task DeleteUserPostAsync(Guid userId, Guid entityId)
        {
            var userPosts = await _dbContext.UserPosts.Where(x => x.UserId == userId).ToListAsync();
            var userPost = userPosts.FirstOrDefault(x => x.Posts.Any(a => a.EntityId == entityId));
            if (userPost != null)
            {
                userPost.Posts = userPost.Posts.Where(x => x.EntityId != entityId).ToList();
                _dbContext.Update(userPost);
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task<Pagination<PostViewModel>> GetUserPostsAsync(Guid userId, int page)
        {
            var limit = 20;
            if (page <= 0)
            {
                page = 1;
            }
            var totalItems = page * limit;
            var userPosts = await PrivateGetUserPostsAsync(userId, page, limit);
            if (userPosts.Count == 0)
            {
                return new Pagination<PostViewModel>(new List<PostViewModel> { }, 0, page, limit);
            }
            if (userPosts.Count == limit)
            {
                totalItems++;
            }
            var ids = userPosts.Select(x => x.EntityId);
            var posts = await _dbContext.Posts.Where(x => ids.Contains(x.Id)).ToListAsync();
            return Pagination<PostViewModel>.GetPagination<Post, PostViewModel>(posts, totalItems, PostMapper.ToView!, page, limit, true);
        }

        private async Task<IList<MiniEntity>> PrivateGetUserPostsAsync(Guid userId, int page, int limit)
        {
            var finalPage = (int)Math.Ceiling(decimal.Divide((page * limit), 100));
            var userPost = await _dbContext.UserPosts.FindAsync(GenerateKeys.GetUserPostId(userId, finalPage, "POST"));

            if (userPost != null)
            {
                _dbContext.Entry(userPost).State = EntityState.Detached;
                var skip = ((page - 1) * limit) % 100;
                return userPost.Posts.OrderByDescending(x => x.CreatedDate).Skip(skip).Take(limit).ToList();
            }
            return new List<MiniEntity> { };
        }
    }
}