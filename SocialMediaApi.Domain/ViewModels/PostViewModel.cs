using SocialMediaApi.Domain.DTOs;
using SocialMediaApi.Domain.Entities.Base;
using SocialMediaApi.Domain.Enums;

namespace SocialMediaApi.Domain.ViewModels
{
    public class PostViewModel : BasePost
    {
        public Guid OwnerId { get; set; }
        public EntityOrigin EntityOrigin { get; set; }
        public ReactionDto Reaction { get; set; } = new ReactionDto();
    }
}