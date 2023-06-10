using SocialMediaApi.Domain.Entities;
using SocialMediaApi.Domain.Entities.Base;
using SocialMediaApi.Domain.Enums;
using SocialMediaApi.Domain.ViewModels;

namespace SocialMediaApi.Domain.Mappers
{
    public struct GroupPostMapper
    {
        public static GroupPostViewModel? ToView(GroupPost? groupPost)
        {
            var viewPost = BaseToView(groupPost);
            if (viewPost == null)
            {
                return default;
            }
            viewPost.GroupId = groupPost!.GroupId;
            viewPost.EntityOrigin = EntityOrigin.None;
            return viewPost;
        }

        public static GroupPostViewModel? ToView(ActiveGroupPost? groupPost)
        {
            var viewPost = BaseToView(groupPost);
            if (viewPost == null)
            {
                return default;
            }
            viewPost.GroupId = groupPost!.GroupId;
            viewPost.EntityOrigin = EntityOrigin.Active;

            return viewPost;
        }

        private static GroupPostViewModel? BaseToView(BasePost? basePost)
        {
            if (basePost == null)
            {
                return default;
            }
            return new GroupPostViewModel
            {
                CreatedDate = basePost.CreatedDate,
                Creator = basePost.Creator,
                Id = basePost.Id,
                LastModifiedDate = basePost.LastModifiedDate,
                Media = basePost.Media,
                Views = basePost.Views,
                TotalComments = basePost.TotalComments,
                Downloads = basePost.Downloads,
                EntityStatus = basePost.EntityStatus,
                Reactions = basePost.Reactions,
                Text = basePost.Text,
                ActionBasedDate = basePost.ActionBasedDate,
            };
        }
    }
}