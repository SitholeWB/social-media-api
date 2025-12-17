namespace SocialMedia.Application;

public record GetUserByIdQuery(Guid UserId) : IQuery<AuthResponse>;