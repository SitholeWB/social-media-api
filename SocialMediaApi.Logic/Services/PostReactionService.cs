using SocialMediaApi.Data;
using SocialMediaApi.Domain.Models.Reactions;
using SocialMediaApi.Domain.ViewModels;
using SocialMediaApi.Interfaces;

namespace SocialMediaApi.Logic.Services
{
    public class PostReactionService : IPostReactionService
    {
        private readonly IEntityDetailsService _entityDetailsService;
        private readonly SocialMediaApiDbContext _dbContext;

        public PostReactionService(IEntityDetailsService entityDetailsService, SocialMediaApiDbContext dbContext)
        {
            _entityDetailsService = entityDetailsService;
            _dbContext = dbContext;
        }

        public async Task<EntityReactionViewModel> AddReactionAsync(AddEntityReactionModel model)
        {
            var userReaction = await _entityDetailsService.AddReactionAsync(model);
            var post = await _dbContext.GroupPosts.FindAsync(userReaction.EntityId);
            if (post != null)
            {
                post.Reactions = userReaction.Summary;
                _dbContext.GroupPosts.Update(post);
                var activePost = await _dbContext.ActiveGroupPosts.FindAsync(userReaction.EntityId);
                if (activePost != null)
                {
                    activePost.Reactions = userReaction.Summary;
                    _dbContext.ActiveGroupPosts.Update(activePost);
                }
                await _dbContext.SaveChangesAsync();
            }

            return userReaction;
        }

        public async Task<EntityReactionViewModel?> DeleteReactionAsync(Guid entityId)
        {
            var userReaction = await _entityDetailsService.DeleteReactionAsync(entityId);
            if (userReaction != null)
            {
                var post = await _dbContext.GroupPosts.FindAsync(userReaction.EntityId);
                if (post != null)
                {
                    post.Reactions = userReaction.Summary;
                    _dbContext.GroupPosts.Update(post);
                    var activePost = await _dbContext.ActiveGroupPosts.FindAsync(userReaction.EntityId);
                    if (activePost != null)
                    {
                        activePost.Reactions = userReaction.Summary;
                        _dbContext.ActiveGroupPosts.Update(activePost);
                    }
                    await _dbContext.SaveChangesAsync();
                }
            }
            return userReaction;
        }
    }
}