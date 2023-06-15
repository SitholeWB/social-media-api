using SocialMediaApi.Data;
using SocialMediaApi.Domain.Models.Reactions;
using SocialMediaApi.Domain.ViewModels;
using SocialMediaApi.Interfaces;

namespace SocialMediaApi.Logic.Services
{
    public class PostReactionService : IPostReactionService
    {
        private readonly IUserReactionService _reactionService;
        private readonly SocialMediaApiDbContext _dbContext;

        public PostReactionService(IUserReactionService reactionService, SocialMediaApiDbContext dbContext)
        {
            _reactionService = reactionService;
            _dbContext = dbContext;
        }

        public async Task<UserReactionViewModel> AddReactionAsync(AddReactionModel model)
        {
            var userReaction = await _reactionService.AddReactionAsync(model);
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

        public async Task<UserReactionViewModel?> DeleteReactionAsync(Guid entityId)
        {
            var userReaction = await _reactionService.DeleteReactionAsync(entityId);
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