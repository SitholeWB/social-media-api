﻿using SocialMediaApi.Domain.Models.Reactions;
using SocialMediaApi.Domain.ViewModels;

namespace SocialMediaApi.Domain.Interfaces
{
    public interface IPostReactionService
    {
        public Task<EntityReactionViewModel> AddReactionAsync(Guid entityId, AddEntityReactionModel model);

        public Task<EntityReactionViewModel?> DeleteReactionAsync(Guid entityId);
    }
}