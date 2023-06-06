using SocialMediaApi.Domain.Entities;
using SocialMediaApi.Domain.ViewModels;

namespace SocialMediaApi.Domain.Mappers
{
    public struct GroupPostMapper
    {
        public static GroupPostViewModel? ToView(GroupPost? groupPost)
        {
            if (groupPost == null)
            {
                return default;
            }
            return new GroupPostViewModel
            {
                CreatedDate = groupPost.CreatedDate,
                Creator = groupPost.Creator,
                Id = groupPost.Id,
                LastModifiedDate = groupPost.LastModifiedDate,
                Media = groupPost.Media,
                Views = groupPost.Views,
                TotalComments = groupPost.TotalComments,
                ThumbnailUrl = groupPost.ThumbnailUrl,
                Downloads = groupPost.Downloads,
                EntityStatus = groupPost.EntityStatus,
                GroupId = groupPost.GroupId,
                Reactions = groupPost.Reactions,
                Text = groupPost.Text,
            };
        }
    }
}