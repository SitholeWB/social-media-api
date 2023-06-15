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
    public class UserReactionService : IUserReactionService
    {
        private readonly SocialMediaApiDbContext _dbContext;
        private readonly IAuthService _authService;
        private readonly EventHandlerContainer _publisher;

        public UserReactionService(SocialMediaApiDbContext dbContext, IAuthService authService, EventHandlerContainer publisher)
        {
            _dbContext = dbContext;
            _authService = authService;
            _publisher = publisher;
        }

        public async Task<UserReactionViewModel> AddReactionAsync(AddReactionModel model)
        {
            if (string.IsNullOrEmpty(model?.Unicode))
            {
                throw new SocialMediaException("Unicode is required.");
            }
            var authUser = await _authService.GetAuthorizedUser();
            var userReaction = await _dbContext.UserReactions.FindAsync(model.EntityId);
            if (userReaction == null)
            {
                var entity = new UserReaction
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
                await _dbContext.AddAsync(entity);
            }
            else
            {
                UpdateUserReaction(userReaction, authUser, model);
            }
            await _dbContext.SaveChangesAsync();
            return GroupPostMapper.ToView(userReaction)!;
        }

        public async Task<UserReactionViewModel?> DeleteReactionAsync(Guid entityId)
        {
            var userReaction = await _dbContext.UserReactions.FindAsync(entityId);
            if (userReaction != null)
            {
                var authUser = await _authService.GetAuthorizedUser();
                var oldReaction = userReaction.Reactions.FirstOrDefault(x => x.Creator.Id == authUser.Id);
                userReaction.Reactions = userReaction.Reactions.Where(x => x.Creator.Id != authUser.Id).ToList();//Remove user reaction
                var oldEmoji = userReaction.Summary.Emojis.FirstOrDefault(x => x.Unicode == oldReaction?.Unicode);
                if (oldEmoji != null)// Should never be null here but you will never know the future developer mind.
                {
                    if (oldEmoji.Count <= 1)
                    {
                        userReaction.Summary.Emojis = userReaction.Summary.Emojis.Where(x => x.Unicode != oldReaction?.Unicode).ToList();
                    }
                    else
                    {
                        oldEmoji.Count--;
                    }
                }
                _dbContext.Update(userReaction);
                await _dbContext.SaveChangesAsync();
            }
            return GroupPostMapper.ToView(userReaction);
        }

        public async Task<UserReactionViewModel?> GetReactionAsync(Guid entityId)
        {
            return GroupPostMapper.ToView(await _dbContext.UserReactions.FindAsync(entityId));
        }

        private void UpdateUserReaction(UserReaction userReaction, BaseUser authUser, AddReactionModel model)
        {
            var oldReaction = userReaction.Reactions.FirstOrDefault(x => x.Creator.Id == authUser.Id);
            userReaction.Reactions = userReaction.Reactions.Where(x => x.Creator.Id != authUser.Id).ToList();//Remove user reaction
            var oldEmoji = userReaction.Summary.Emojis.FirstOrDefault(x => x.Unicode == oldReaction?.Unicode);
            if (oldEmoji != null)// Should never be null here but you will never know the future developer mind.
            {
                if (oldEmoji.Count <= 1)
                {
                    userReaction.Summary.Emojis = userReaction.Summary.Emojis.Where(x => x.Unicode != oldReaction?.Unicode).ToList();
                }
                else
                {
                    oldEmoji.Count--;
                }
            }

            userReaction.Reactions.Add(new Reaction
            {
                Creator = authUser,
                Unicode = model.Unicode
            });
            userReaction.Summary.ReactionsCount++;
            var emoji = userReaction.Summary.Emojis.FirstOrDefault(x => x.Unicode == model.Unicode);
            if (emoji == null)
            {
                userReaction.Summary.Emojis.Add(new Emoji
                {
                    Count = 1,
                    Unicode = model.Unicode,
                });
            }
            else
            {
                emoji.Count++;
            }

            userReaction.LastModifiedDate = DateTimeOffset.UtcNow;
            _dbContext.Update(userReaction);
        }
    }
}