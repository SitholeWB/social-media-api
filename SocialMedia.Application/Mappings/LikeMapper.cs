namespace SocialMedia.Application;

public static class LikeMapper
{
    public static LikeDto ToDto(this Like like)
    {
        return new LikeDto
        {
            Id = like.Id,
            UserId = like.UserId,
            PostId = like.PostId,
            CommentId = like.CommentId,
            Emoji = like.Emoji
        };
    }
}