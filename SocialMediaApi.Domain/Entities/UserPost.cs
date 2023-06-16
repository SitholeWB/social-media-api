using SocialMediaApi.Domain.Entities.JsonEntities;
using System.ComponentModel.DataAnnotations;

namespace SocialMediaApi.Domain.Entities
{
    public class UserPost
    {
        [Key]
        public string Id { get; set; } = string.Empty;

        public List<MiniEntity> Posts { get; set; } = new List<MiniEntity>();
    }
}