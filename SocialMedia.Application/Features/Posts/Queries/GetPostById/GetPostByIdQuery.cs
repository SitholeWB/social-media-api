
namespace SocialMedia.Application;

public record GetPostByIdQuery(Guid Id) : IQuery<PostDto?>
{
    public Guid? UserId { get; set; }
}
