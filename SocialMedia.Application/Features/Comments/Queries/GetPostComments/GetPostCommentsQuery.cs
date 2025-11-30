using SocialMedia.Domain.ReadModels;

namespace SocialMedia.Application;

public record GetPostCommentsQuery(Guid PostId, int PageNumber = 1, int PageSize = 10) : IQuery<PagedResult<CommentReadDto>>;
