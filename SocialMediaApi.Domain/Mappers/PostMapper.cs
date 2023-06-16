using SocialMediaApi.Domain.Entities;
using SocialMediaApi.Domain.Entities.Base;
using SocialMediaApi.Domain.Enums;
using SocialMediaApi.Domain.ViewModels;

namespace SocialMediaApi.Domain.Mappers
{
    public struct PostMapper
    {
        public static PostViewModel? ToView(Post? post)
        {
            var viewPost = BaseToView(post);
            if (viewPost == null)
            {
                return default;
            }
            viewPost.GroupId = post!.GroupId;
            viewPost.EntityOrigin = EntityOrigin.None;
            return viewPost;
        }

        public static PostViewModel? ToView(ActivePost? post)
        {
            var viewPost = BaseToView(post);
            if (viewPost == null)
            {
                return default;
            }
            viewPost.GroupId = post!.GroupId;
            viewPost.EntityOrigin = EntityOrigin.Active;

            return viewPost;
        }

        private static PostViewModel? BaseToView(BasePost? basePost)
        {
            if (basePost == null)
            {
                return default;
            }
            return new PostViewModel
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

        public static PostCommentViewModel? ToView(PostComment? postComment)
        {
            if (postComment == null)
            {
                return default;
            }
            return new PostCommentViewModel
            {
                CreatedDate = postComment.CreatedDate,
                Creator = postComment.Creator,
                Id = postComment.Id,
                LastModifiedDate = postComment.LastModifiedDate,
                Media = postComment.Media,
                Views = postComment.Views,
                TotalComments = postComment.TotalComments,
                Downloads = postComment.Downloads,
                PostId = postComment.PostId,
                Rank = postComment.Rank,
                Reactions = postComment.Reactions,
                Text = postComment.Text,
                ActionBasedDate = postComment.ActionBasedDate,
            };
        }

        public static EntityReactionViewModel? ToView(EntityDetails? reaction)
        {
            if (reaction == null)
            {
                return default;
            }
            return new EntityReactionViewModel
            {
                CreatedDate = reaction.CreatedDate,
                EntityId = reaction.EntityId,
                LastModifiedDate = reaction.LastModifiedDate,
                Reactions = reaction.Reactions,
                Summary = reaction.Summary,
            };
        }
    }
}