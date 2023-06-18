using SocialMediaApi.Domain.DTOs;
using SocialMediaApi.Domain.Entities;
using SocialMediaApi.Domain.Entities.Base;
using SocialMediaApi.Domain.Entities.JsonEntities;
using SocialMediaApi.Domain.Enums;
using SocialMediaApi.Domain.ViewModels;

namespace SocialMediaApi.Domain.Mappers
{
    public class PostMapper
    {
        public static PostViewModel? ToView(Post? post, IList<MiniReaction> reactions)
        {
            var viewPost = BaseToView(post);
            if (viewPost == null)
            {
                return default;
            }
            viewPost.OwnerId = post!.OwnerId;
            viewPost.EntityOrigin = EntityOrigin.None;
            var reaction = reactions.FirstOrDefault(x => x.EntityId == post.Id);
            if (reaction != null)
            {
                viewPost.Reaction = new ReactionDto
                {
                    Reacted = true,
                    Unicode = reaction.Unicode,
                };
            }
            return viewPost;
        }

        public static PostViewModel? ToView(ActivePost? post, IList<MiniReaction> reactions)
        {
            var viewPost = BaseToView(post);
            if (viewPost == null)
            {
                return default;
            }
            viewPost.OwnerId = post!.OwnerId;
            viewPost.EntityOrigin = EntityOrigin.Active;
            var reaction = reactions.FirstOrDefault(x => x.EntityId == post.Id);
            if (reaction != null)
            {
                viewPost.Reaction = new ReactionDto
                {
                    Reacted = true,
                    Unicode = reaction.Unicode,
                };
            }
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

        public static CommentViewModel? ToView(Comment? comment, IList<MiniReaction> reactions)
        {
            if (comment == null)
            {
                return default;
            }
            var reaction = reactions.FirstOrDefault(x => x.EntityId == comment.Id);
            var reactionDto = new ReactionDto();
            if (reaction != null)
            {
                reactionDto.Reacted = true;
                reactionDto.Unicode = reaction.Unicode;
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
                PostId = comment.PostId,
                Rank = comment.Rank,
                Reactions = comment.Reactions,
                Text = comment.Text,
                ActionBasedDate = comment.ActionBasedDate,
                Reaction = reactionDto
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