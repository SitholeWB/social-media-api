using Microsoft.EntityFrameworkCore;
using Pagination.EntityFrameworkCore.Extensions;
using SocialMediaApi.Data;
using SocialMediaApi.Domain.Common;
using SocialMediaApi.Domain.Entities;
using SocialMediaApi.Domain.Entities.JsonEntities;
using SocialMediaApi.Domain.Models.UserPosts;
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
            var userPost = await _dbContext.UserPosts.FirstOrDefaultAsync(x => x.UserId == userId && x.Posts.Any(a => a.EntityId == entityId));
            if (userPost != null)
            {
                userPost.Posts = userPost.Posts.Where(x => x.EntityId != entityId).ToList();
                _dbContext.Update(userPost);
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task<Pagination<MiniEntity>> GetUserPostsAsync(Guid userId, int page, int limit)
        {
            var finalPage = ((page * limit) / 100) + 1;
            var userPost = await _dbContext.UserPosts.FindAsync(GenerateKeys.GetUserPostId(userId, finalPage, "POST"));

            if (userPost != null)
            {
                _dbContext.Entry(userPost).State = EntityState.Detached;
                var skip = (page * limit) % 100;
                return new Pagination<MiniEntity>(userPost.Posts.Skip(skip).Take(limit), userPost.Posts.Count + 1, page, limit);
            }
            return new Pagination<MiniEntity>(new List<MiniEntity> { }, 0, page, limit);
        }
    }
}