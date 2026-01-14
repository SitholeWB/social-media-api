namespace SocialMedia.Files.API;

public interface IFileService
{
    Task<(Guid? UserFileId, string Url)> UploadFileAsync(string shardKey, string userId, IFormFile file, string fileId = "", CancellationToken token = default);

    Task<(Stream Stream, string ContentType, string FileName)> DownloadFileAsync(string shardKey, string? userId, Guid userFileId, CancellationToken token);

    Task DeleteFileAsync(string shardKey, string userId, Guid userFileId, CancellationToken token);
}