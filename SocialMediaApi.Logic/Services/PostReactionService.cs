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
            var entityReaction = await _entityDetailsService.AddReactionAsync(model);
            var post = await _dbContext.GroupPosts.FindAsync(entityReaction.EntityId);
            if (post != null)
            {
                post.Reactions = entityReaction.Summary;
                _dbContext.GroupPosts.Update(post);
                var activePost = await _dbContext.ActiveGroupPosts.FindAsync(entityReaction.EntityId);
                if (activePost != null)
                {
                    activePost.Reactions = entityReaction.Summary;
                    _dbContext.ActiveGroupPosts.Update(activePost);
                }
                await _dbContext.SaveChangesAsync();
            }

            return entityReaction;
        }

        public async Task<EntityReactionViewModel?> DeleteReactionAsync(Guid entityId)
        {
            var entityReaction = await _entityDetailsService.DeleteReactionAsync(entityId);
            if (entityReaction != null)
            {
                var post = await _dbContext.GroupPosts.FindAsync(entityReaction.EntityId);
                if (post != null)
                {
                    post.Reactions = entityReaction.Summary;
                    _dbContext.GroupPosts.Update(post);
                    var activePost = await _dbContext.ActiveGroupPosts.FindAsync(entityReaction.EntityId);
                    if (activePost != null)
                    {
                        activePost.Reactions = entityReaction.Summary;
                        _dbContext.ActiveGroupPosts.Update(activePost);
                    }
                    await _dbContext.SaveChangesAsync();
                }
            }
            return entityReaction;
        }
    }
}