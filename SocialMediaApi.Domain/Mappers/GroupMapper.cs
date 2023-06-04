using SocialMediaApi.Domain.Entities;
using SocialMediaApi.Domain.ViewModels;

namespace SocialMediaApi.Domain.Mappers
{
    public struct GroupMapper
    {
        public static GroupViewModel? ToView(Group? group)
        {
            if (group == null)
            {
                return default;
            }
            return new GroupViewModel
            {
                CreatedDate = group.CreatedDate,
                Creator = group.Creator,
                Description = group.Description,
                Id = group.Id,
                LastModifiedDate = group.LastModifiedDate,
                Name = group.Name,
            };
        }
    }
}