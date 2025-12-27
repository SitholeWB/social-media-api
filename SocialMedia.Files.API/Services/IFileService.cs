namespace SocialMedia.Files.API;

public interface IFileService
{
    Task<(Guid UserFileId, string Url)> UploadFileAsync(string shardKey, string userId, IFormFile file);

    Task<(Stream Stream, string ContentType, string FileName)> DownloadFileAsync(string shardKey, string? userId, Guid userFileId);

    Task DeleteFileAsync(string shardKey, string userId, Guid userFileId);
}