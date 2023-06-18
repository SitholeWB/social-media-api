using SocialMediaApi.Domain.Entities.Base;
using SocialMediaApi.Domain.Enums;

namespace SocialMediaApi.Domain.ViewModels
{
    public class PostViewModel : BasePost
    {
        public Guid OwnerId { get; set; }
        public EntityOrigin EntityOrigin { get; set; }
    }
}