
namespace SocialMedia.Application;

public record GetPostByIdQuery(Guid Id) : IQuery<PostDto?>;
