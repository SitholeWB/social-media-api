using SocialMediaApi.Domain.Entities;

namespace SocialMediaApi.Domain.Events.Groups
{
    public class DeleteGroupEvent
    {
        public Group? Group { get; set; }
    }
}