﻿using SocialMediaApi.Data;
using SocialMediaApi.Domain.Models.Reactions;
using SocialMediaApi.Domain.ViewModels;
using SocialMediaApi.Domain.Interfaces;

namespace SocialMediaApi.Domain.Logic.Services
{
    public class PostReactionService : IPostReactionService
    {
        private readonly IEntityDetailsService _entityDetailsService;
        private readonly IUserDetailsService _userDetailsService;
        private readonly SocialMediaApiDbContext _dbContext;

        public PostReactionService(IEntityDetailsService entityDetailsService, SocialMediaApiDbContext dbContext, IUserDetailsService userDetailsService)
        {
            _entityDetailsService = entityDetailsService;
            _dbContext = dbContext;
            _userDetailsService = userDetailsService;
        }

        public async Task<EntityReactionViewModel> AddReactionAsync(Guid entityId, AddEntityReactionModel model)
        {
            var entityReaction = await _entityDetailsService.AddReactionAsync(entityId, model);
            var post = await _dbContext.Posts.FindAsync(entityReaction.EntityId);
            if (post != null)
            {
                post.Reactions = entityReaction.Summary;
                _dbContext.Posts.Update(post);
                var activePost = await _dbContext.ActivePosts.FindAsync(entityReaction.EntityId);
                if (activePost != null)
                {
                    activePost.Reactions = entityReaction.Summary;
                    _dbContext.ActivePosts.Update(activePost);
                }
                await _dbContext.SaveChangesAsync();
                await _userDetailsService.AddPostReactionAsync(entityId, model);
            }

            return entityReaction;
        }

        public async Task<EntityReactionViewModel?> DeleteReactionAsync(Guid entityId)
        {
            var entityReaction = await _entityDetailsService.DeleteReactionAsync(entityId);
            if (entityReaction != null)
            {
                var post = await _dbContext.Posts.FindAsync(entityReaction.EntityId);
                if (post != null)
                {
                    post.Reactions = entityReaction.Summary;
                    _dbContext.Posts.Update(post);
                    var activePost = await _dbContext.ActivePosts.FindAsync(entityReaction.EntityId);
                    if (activePost != null)
                    {
                        activePost.Reactions = entityReaction.Summary;
                        _dbContext.ActivePosts.Update(activePost);
                    }
                    await _dbContext.SaveChangesAsync();
                }
                await _userDetailsService.DeletePostReactionAsync(entityId);
            }
            return entityReaction;
        }
    }
}