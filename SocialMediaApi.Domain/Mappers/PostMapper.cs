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

        public static CommentViewModel? ToView(Comment? comment)
        {
            if (comment == null)
            {
                return default;
            }
            return new CommentViewModel
            {
                CreatedDate = comment.CreatedDate,
                Creator = comment.Creator,
                Id = comment.Id,
                LastModifiedDate = comment.LastModifiedDate,
                Media = comment.Media,
                Views = comment.Views,
                TotalComments = comment.TotalComments,
                Downloads = comment.Downloads,
                PostId = comment.PostId,
                Rank = comment.Rank,
                Reactions = comment.Reactions,
                Text = comment.Text,
                ActionBasedDate = comment.ActionBasedDate,
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