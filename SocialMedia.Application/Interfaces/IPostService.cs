namespace SocialMedia.Application;

public interface IPostService
{
    Task<bool> DeletePostAsync(Guid id, CancellationToken cancellationToken);
}