using Microsoft.EntityFrameworkCore;
using Pagination.EntityFrameworkCore.Extensions;
using SocialMediaApi.Data;
using SocialMediaApi.Domain.Entities;
using SocialMediaApi.Domain.Entities.JsonEntities;
using SocialMediaApi.Domain.Enums;
using SocialMediaApi.Domain.Events;
using SocialMediaApi.Domain.Exceptions;
using SocialMediaApi.Domain.Mappers;
using SocialMediaApi.Domain.Models.GroupPost;
using SocialMediaApi.Domain.ViewModels;
using SocialMediaApi.Interfaces;
using SocialMediaApi.Logic.EventHandlers;

namespace SocialMediaApi.Logic.Services
{
    public class GroupPostService : IGroupPostService
    {
        private readonly SocialMediaApiDbContext _dbContext;
        private readonly IAuthService _authService;
        private readonly EventHandlerContainer _publisher;

        public GroupPostService(SocialMediaApiDbContext dbContext, IAuthService authService, EventHandlerContainer publisher)
        {
            _dbContext = dbContext;
            _authService = authService;
            _publisher = publisher;
        }

        public async Task<GroupPostViewModel> AddGroupPostAsync(Guid groupId, AddGroupPostModel model)
        {
            if (string.IsNullOrEmpty(model?.Text))
            {
                throw new SocialMediaException("Text is required.");
            }
            if (model?.Media == null)
            {
                throw new SocialMediaException("Media explicit definition is required.");
            }
            var authUser = await _authService.GetAuthorizedUser();
            var entity = new GroupPost
            {
                CreatedDate = DateTimeOffset.UtcNow,
                Id = Guid.NewGuid(),
                EntityStatus = EntityStatus.Ready,
                LastModifiedDate = DateTimeOffset.UtcNow,
                Creator = authUser,
                Text = model!.Text,
                Downloads = 0,
                GroupId = groupId,
                Reactions = new Reaction
                {
                    Emojis = new List<Emoji>(),
                    ReactionsCount = 0
                },
                ThumbnailUrl = model?.ThumbnailUrl ?? "",
                TotalComments = 0,
                Views = 0,
                Media = model!.Media
            };
            var addedEntity = await _dbContext.AddAsync(entity);
            await _dbContext.SaveChangesAsync();
            await _publisher.PublishAsync(new AddGroupPostEvent { GroupPost = addedEntity.Entity });
            return GroupPostMapper.ToView(addedEntity.Entity)!;
        }

        public async Task DeleteGroupPostAsync(Guid groupId, Guid id)
        {
            var groupPost = await _dbContext.GroupPosts.FirstOrDefaultAsync(x => x.Id == id && x.GroupId == groupId) ?? throw new SocialMediaException("No Post found for given Id & groupId.");
            var authUser = await _authService.GetAuthorizedUser();
            if (!authUser.Id.Equals(groupPost.Creator.Id))
            {
                throw new SocialMediaException("Post can only be deleted by the creator.");
            }
            groupPost.EntityStatus = EntityStatus.Deleted;
            _dbContext.GroupPosts.Update(groupPost);
            await _dbContext.SaveChangesAsync();
            await _publisher.PublishAsync(new DeleteGroupPostEvent { GroupPost = groupPost });
        }

        public Task<GroupPostViewModel> GetGroupPostAsync(Guid groupId, Guid id)
        {
            throw new NotImplementedException();
        }

        public Task<Pagination<GroupPostViewModel>> GetGroupPostsAsync(Guid groupId, int page = 1, int limit = 20)
        {
            throw new NotImplementedException();
        }

        public Task<GroupPostViewModel> UpdateGroupPostAsync(Guid groupId, Guid id, UpdateGroupPostModel model)
        {
            throw new NotImplementedException();
        }
    }
}