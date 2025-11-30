
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
            FileUrl = comment.File?.Url,
            CreatedAt = comment.CreatedAt
        };
    }

    public static Comment ToEntity(this CreateCommentDto dto)
    {
        return new Comment
        {
            Content = dto.Content,
            PostId = dto.PostId,
            AuthorId = dto.AuthorId,
            FileId = dto.FileId
        };
    }
}
