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
            GroupId = post.GroupId,
            Media = post.Media,
            CreatedAt = post.CreatedAt,
            AdminTags = post.AdminTags,
            Tags = post.Tags,
            LikeCount = post.Likes?.Count ?? 0,
            CommentCount = post.Comments?.Count ?? 0,
            StatusFullScreen = post.StatusFullScreen
        };
    }

    public static Post ToEntity(this CreatePostDto dto)
    {
        return new Post
        {
            Title = dto.Title,
            Content = dto.Content,
            AuthorId = dto.AuthorId,
            GroupId = dto.GroupId,
            StatusFullScreen = dto.StatusFullScreen,
            Media = dto.Media,
            Tags = dto.Tags?.Select(x => new TagDto { Text = x, Topic = "Not-Set" })?.ToList() ?? [],
            AdminTags = dto.AdminTags?.Select(x => new TagDto { Text = x, Topic = "Not-Set" })?.ToList() ?? []
        };
    }
}