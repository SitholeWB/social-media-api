using SocialMediaApi.Data;
using SocialMediaApi.Domain.Entities;
using SocialMediaApi.Domain.Entities.Base;
using SocialMediaApi.Domain.Entities.JsonEntities;
using SocialMediaApi.Domain.Exceptions;
using SocialMediaApi.Domain.JsonEntities;
using SocialMediaApi.Domain.Mappers;
using SocialMediaApi.Domain.Models.Reactions;
using SocialMediaApi.Domain.ViewModels;
using SocialMediaApi.Interfaces;
using SocialMediaApi.Logic.EventHandlers;

namespace SocialMediaApi.Logic.Services
{
    public class EntityDetailsService : IEntityDetailsService
    {
        private readonly SocialMediaApiDbContext _dbContext;
        private readonly IAuthService _authService;
        private readonly EventHandlerContainer _publisher;

        public EntityDetailsService(SocialMediaApiDbContext dbContext, IAuthService authService, EventHandlerContainer publisher)
        {
            _dbContext = dbContext;
            _authService = authService;
            _publisher = publisher;
        }

        public async Task<EntityReactionViewModel> AddReactionAsync(AddEntityReactionModel model)
        {
            if (string.IsNullOrEmpty(model?.Unicode))
            {
                throw new SocialMediaException("Unicode is required.");
            }
            var authUser = await _authService.GetAuthorizedUser();
            var entityReaction = await _dbContext.EntityDetails.FindAsync(model.EntityId);
            if (entityReaction == null)
            {
                entityReaction = new EntityDetails
                {
                    CreatedDate = DateTimeOffset.UtcNow,
                    LastModifiedDate = DateTimeOffset.UtcNow,
                    EntityId = model.EntityId,
                    Reactions = new List<Reaction>
                    {
                        new Reaction
                        {
                            Creator = authUser,
                            Unicode = model.Unicode,
                        }
                    },
                    Summary = new ReactionSummary
                    {
                        ReactionsCount = 1,
                        Emojis = new List<Emoji>
                        {
                            new Emoji
                            {
                                Unicode = model.Unicode,
                                Count = 1
                            }
                        }
                    }
                };
                await _dbContext.AddAsync(entityReaction);
            }
            else
            {
                UpdateUserReaction(entityReaction, authUser, model);
            }
            await _dbContext.SaveChangesAsync();
            return GroupPostMapper.ToView(entityReaction)!;
        }

        public async Task<EntityReactionViewModel?> DeleteReactionAsync(Guid entityId)
        {
            var entityReaction = await _dbContext.EntityDetails.FindAsync(entityId);
            if (entityReaction != null)
            {
                var authUser = await _authService.GetAuthorizedUser();
                var oldReaction = entityReaction.Reactions.FirstOrDefault(x => x.Creator.Id == authUser.Id);
                entityReaction.Reactions = entityReaction.Reactions.Where(x => x.Creator.Id != authUser.Id).ToList();//Remove user reaction
                var oldEmoji = entityReaction.Summary.Emojis.FirstOrDefault(x => x.Unicode == oldReaction?.Unicode);
                if (oldEmoji != null)// Should never be null here but you will never know the future developer mind.
                {
                    if (oldEmoji.Count <= 1)
                    {
                        entityReaction.Summary.Emojis = entityReaction.Summary.Emojis.Where(x => x.Unicode != oldReaction?.Unicode).ToList();
                    }
                    else
                    {
                        oldEmoji.Count--;
                    }
                }
                _dbContext.Update(entityReaction);
                await _dbContext.SaveChangesAsync();
            }
            return GroupPostMapper.ToView(entityReaction);
        }

        public async Task<EntityReactionViewModel?> GetReactionAsync(Guid entityId)
        {
            return GroupPostMapper.ToView(await _dbContext.EntityDetails.FindAsync(entityId));
        }

        private void UpdateUserReaction(EntityDetails entityReaction, BaseUser authUser, AddEntityReactionModel model)
        {
            var oldReaction = entityReaction.Reactions.FirstOrDefault(x => x.Creator.Id == authUser.Id);
            entityReaction.Reactions = entityReaction.Reactions.Where(x => x.Creator.Id != authUser.Id).ToList();//Remove user reaction
            var oldEmoji = entityReaction.Summary.Emojis.FirstOrDefault(x => x.Unicode == oldReaction?.Unicode);
            if (oldEmoji != null)
            {
                if (oldEmoji.Count <= 1)
                {
                    entityReaction.Summary.Emojis = entityReaction.Summary.Emojis.Where(x => x.Unicode != oldReaction?.Unicode).ToList();
                }
                else
                {
                    oldEmoji.Count--;
                }
            }

            entityReaction.Reactions.Add(new Reaction
            {
                Creator = authUser,
                Unicode = model.Unicode
            });

            var emoji = entityReaction.Summary.Emojis.FirstOrDefault(x => x.Unicode == model.Unicode);
            if (emoji == null)
            {
                entityReaction.Summary.Emojis.Add(new Emoji
                {
                    Count = 1,
                    Unicode = model.Unicode,
                });
            }
            else
            {
                emoji.Count++;
            }
            entityReaction.Summary.ReactionsCount = entityReaction.Reactions.Count;
            entityReaction.LastModifiedDate = DateTimeOffset.UtcNow;
            _dbContext.Update(entityReaction);
        }
    }
}