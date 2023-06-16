using SocialMediaApi.Data;
using SocialMediaApi.Domain.Models.Reactions;
using SocialMediaApi.Domain.ViewModels;
using SocialMediaApi.Interfaces;

namespace SocialMediaApi.Logic.Services
{
    public class CommentReactionService : ICommentReactionService
    {
        private readonly IEntityDetailsService _entityDetailsService;
        private readonly SocialMediaApiDbContext _dbContext;

        public CommentReactionService(IEntityDetailsService entityDetailsService, SocialMediaApiDbContext dbContext)
        {
            _entityDetailsService = entityDetailsService;
            _dbContext = dbContext;
        }

        public async Task<EntityReactionViewModel> AddReactionAsync(AddEntityReactionModel model)
        {
            var entityReaction = await _entityDetailsService.AddReactionAsync(model);
            var post = await _dbContext.Comments.FindAsync(entityReaction.EntityId);
            if (post != null)
            {
                post.Reactions = entityReaction.Summary;
                _dbContext.Comments.Update(post);
                await _dbContext.SaveChangesAsync();
            }

            return entityReaction;
        }

        public async Task<EntityReactionViewModel?> DeleteReactionAsync(Guid entityId)
        {
            var entityReaction = await _entityDetailsService.DeleteReactionAsync(entityId);
            if (entityReaction != null)
            {
                var post = await _dbContext.Comments.FindAsync(entityReaction.EntityId);
                if (post != null)
                {
                    post.Reactions = entityReaction.Summary;
                    _dbContext.Comments.Update(post);
                    await _dbContext.SaveChangesAsync();
                }
            }
            return entityReaction;
        }
    }
}