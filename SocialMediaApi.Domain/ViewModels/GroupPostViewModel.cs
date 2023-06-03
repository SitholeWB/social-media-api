using SocialMediaApi.Domain.Entities.Base;

namespace SocialMediaApi.Domain.ViewModels
{
    public class GroupPostViewModel : BasePost
    {
        public Guid GroupId { get; set; }
    }
}