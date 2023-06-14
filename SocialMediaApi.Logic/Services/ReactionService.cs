using Microsoft.EntityFrameworkCore;
using Pagination.EntityFrameworkCore.Extensions;
using SocialMediaApi.Data;
using SocialMediaApi.Domain.Entities;
using SocialMediaApi.Domain.Exceptions;
using SocialMediaApi.Domain.Mappers;
using SocialMediaApi.Domain.Models.Reactions;
using SocialMediaApi.Domain.ViewModels;
using SocialMediaApi.Interfaces;
using SocialMediaApi.Logic.EventHandlers;

namespace SocialMediaApi.Logic.Services
{
    public class ReactionService : IReactionService
    {
        private readonly SocialMediaApiDbContext _dbContext;
        private readonly IAuthService _authService;
        private readonly EventHandlerContainer _publisher;

        public ReactionService(SocialMediaApiDbContext dbContext, IAuthService authService, EventHandlerContainer publisher)
        {
            _dbContext = dbContext;
            _authService = authService;
            _publisher = publisher;
        }

        public async Task<ReactionViewModel> AddReactionAsync(AddReactionModel model)
        {
            if (string.IsNullOrEmpty(model?.Unicode))
            {
                throw new SocialMediaException("Unicode is required.");
            }
            var authUser = await _authService.GetAuthorizedUser();
            var reaction = await _dbContext.Reactions.FirstOrDefaultAsync(x => x.EntityId == model.EntityId && x.UserId == authUser.Id);
            if (reaction == null)
            {
                var entity = new Reaction
                {
                    CreatedDate = DateTimeOffset.UtcNow,
                    Id = Guid.NewGuid(),
                    LastModifiedDate = DateTimeOffset.UtcNow,
                    Creator = authUser,
                    EntityId = model.EntityId,
                    UserId = authUser.Id,
                    Unicode = model.Unicode,
                };
                var addedEntity = await _dbContext.AddAsync(entity);
                await _dbContext.SaveChangesAsync();
                return GroupPostMapper.ToView(addedEntity.Entity)!;
            }

            reaction.Unicode = model.Unicode;
            reaction.LastModifiedDate = DateTimeOffset.UtcNow;
            _dbContext.Update(reaction);
            await _dbContext.SaveChangesAsync();
            return GroupPostMapper.ToView(reaction)!;
        }

        public async Task DeleteReactionAsync(Guid entityId, Guid id)
        {
            var reaction = await _dbContext.Reactions.FindAsync(id);
            if (reaction == null)
            {
                var authUser = await _authService.GetAuthorizedUser();
                reaction = await _dbContext.Reactions.FirstOrDefaultAsync(x => x.EntityId == entityId && x.UserId == authUser.Id);
            }
            if (reaction != null)
            {
                _dbContext.Remove(reaction);
                await _dbContext.SaveChangesAsync();
            }
        }

        public async Task<Pagination<ReactionViewModel>> GetReactionsAsync(Guid entityId, int page = 1, int limit = 20)
        {
            return await _dbContext.AsPaginationAsync<Reaction, ReactionViewModel>(page, limit, x => x.EntityId == entityId, GroupPostMapper.ToView!);
        }
    }
}