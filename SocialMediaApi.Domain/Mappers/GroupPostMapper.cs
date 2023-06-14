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

        public static GroupPostCommentViewModel? ToView(GroupPostComment? groupPostComment)
        {
            if (groupPostComment == null)
            {
                return default;
            }
            return new GroupPostCommentViewModel
            {
                CreatedDate = groupPostComment.CreatedDate,
                Creator = groupPostComment.Creator,
                Id = groupPostComment.Id,
                LastModifiedDate = groupPostComment.LastModifiedDate,
                Media = groupPostComment.Media,
                Views = groupPostComment.Views,
                TotalComments = groupPostComment.TotalComments,
                Downloads = groupPostComment.Downloads,
                GroupPostId = groupPostComment.GroupPostId,
                Rank = groupPostComment.Rank,
                Reactions = groupPostComment.Reactions,
                Text = groupPostComment.Text,
                ActionBasedDate = groupPostComment.ActionBasedDate,
            };
        }

        public static ReactionViewModel? ToView(Reaction? reaction)
        {
            if (reaction == null)
            {
                return default;
            }
            return new ReactionViewModel
            {
                Unicode = reaction.Unicode,
                EntityId = reaction.EntityId,
                Creator = reaction.Creator
            };
        }
    }
}