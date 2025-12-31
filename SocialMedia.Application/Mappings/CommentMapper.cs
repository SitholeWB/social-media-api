namespace SocialMedia.Application;

public static class CommentMapper
{
    public static CommentDto ToDto(this Comment comment)
    {
        return new CommentDto
        {
            Id = comment.Id,
            Content = comment.Content,
            PostId = comment.PostId,
            AuthorId = comment.AuthorId,
            Media = comment.Media,
            CreatedAt = comment.CreatedAt,
            AdminTags = comment.AdminTags,
            Tags = comment.Tags
        };
    }

    public static Comment ToEntity(this CreateCommentDto dto)
    {
        return new Comment
        {
            Content = dto.Content,
            PostId = dto.PostId,
            AuthorId = dto.AuthorId,
            Media = dto.Media,
            AdminTags = dto.AdminTags?.Select(x => new TagDto { Text = x, Topic = "Not-Set" })?.ToList() ?? [],
            Tags = dto.Tags?.Select(x => new TagDto { Text = x, Topic = "Not-Set" })?.ToList() ?? [],
        };
    }
}