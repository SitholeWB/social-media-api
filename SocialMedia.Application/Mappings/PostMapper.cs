namespace SocialMedia.Application;

public static class PostMapper
{
    public static PostDto ToDto(this Post post)
    {
        return new PostDto
        {
            Id = post.Id,
            Title = post.Title,
            Content = post.Content,
            AuthorId = post.AuthorId,
            FileUrl = post.File?.Url,
            CreatedAt = post.CreatedAt,
            AdminTags = post.AdminTags,
            Tags = post.Tags,
            LikeCount = post.Likes?.Count ?? 0,
            CommentCount = post.Comments?.Count ?? 0,
        };
    }

    public static Post ToEntity(this CreatePostDto dto)
    {
        return new Post
        {
            Title = dto.Title,
            Content = dto.Content,
            AuthorId = dto.AuthorId,
            FileId = dto.FileId,
            Tags = dto.Tags,
            AdminTags = dto.AdminTags
        };
    }
}