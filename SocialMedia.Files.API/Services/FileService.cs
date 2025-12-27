using Files.EntityFrameworkCore.Extensions;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;

namespace SocialMedia.Files.API;

public class FileService : IFileService
{
    private readonly FileDbContext _context;
    private readonly ILogger<FileService> _logger;

    public FileService(FileDbContext context, ILogger<FileService> logger)
    {
        _context = context;
        _logger = logger;
    }

    public async Task<(Guid UserFileId, string Url)> UploadFileAsync(string shardKey, string userId, IFormFile file)
    {
        var contentType = string.IsNullOrWhiteSpace(file.ContentType) ? "application/octet-stream" : file.ContentType;
        // 1. Calculate Hash
        using var stream = file.OpenReadStream();
        using var sha256 = SHA256.Create();
        var hashBytes = await sha256.ComputeHashAsync(stream);
        var hash = Convert.ToHexString(hashBytes);

        stream.Position = 0; // Reset stream for reading

        // 2. Check Deduplication
        var existingUserFile = await _context.UserFiles
            .FirstOrDefaultAsync(u => u.Hash == hash);

        Guid fileId;

        if (existingUserFile != null)
        {
            fileId = existingUserFile.FileId;
            _logger.LogInformation("Duplicate file found with hash {Hash}. Reusing FileId {FileId}", hash, fileId);
        }
        else
        {
            // 3. Save new file
            var result = await _context.SaveFileAsync<StoredFile>(stream, file.FileName, contentType);
            fileId = result.Id; // result.Id is the Group ID (StoredFile.FileId)
        }

        // 4. Create UserFile
        var userFile = new UserFile
        {
            Id = Guid.NewGuid(),
            UserId = userId,
            FileId = fileId,
            FileName = file.FileName,
            Hash = hash,
            DatabaseName = shardKey,
            MimeType = contentType,
            CreatedOn = DateTime.UtcNow
        };

        _context.UserFiles.Add(userFile);
        await _context.SaveChangesAsync();

        return (userFile.Id, $"/api/{shardKey}/files/{userFile.Id}");
    }

    public async Task<(Stream Stream, string ContentType, string FileName)> DownloadFileAsync(string shardKey, string? userId, Guid userFileId)
    {
        var userFile = await _context.UserFiles.FindAsync(userFileId);
        if (userFile == null)
            throw new FileNotFoundException("File not found");

        // Simple ownership check or public access if userId is null/empty?
        // Assumption: If userId is provided, we check. If not, maybe we allow public? User request
        // says "used in future for download", implies ownership. But for now, let's allow if the
        // knowledge of UUID is enough, OR restricted? Let's enforce ownership if userId is present.
        if (!string.IsNullOrEmpty(userId) && userFile.UserId != userId)
        {
            // For now, let's verify ownership. If checking against the JWT. If public download is
            // needed, we will skip this check in Controller or pass null. But Wait, "Get identifier
            // from JWT" implies strict link. I'll throw UnauthorizedAccessException if userId mismatches.
            throw new UnauthorizedAccessException("User does not own this file");
        }

        var stream = new MemoryStream();
        await _context.DownloadFileToStreamAsync<StoredFile>(userFile.FileId, stream);
        stream.Position = 0;

        return (stream, userFile.MimeType ?? "application/octet-stream", userFile.FileName);
    }

    public async Task DeleteFileAsync(string shardKey, string userId, Guid userFileId)
    {
        var userFile = await _context.UserFiles.FindAsync(userFileId);
        if (userFile == null)
            return; // Already gone

        if (userFile.UserId != userId)
            throw new UnauthorizedAccessException("User does not own this file");

        // Check reference count
        var count = await _context.UserFiles.CountAsync(u => u.FileId == userFile.FileId);

        _context.UserFiles.Remove(userFile);
        await _context.SaveChangesAsync();

        if (count <= 1)
        {
            // Last reference, delete actual file
            // Note: We removed userFile above, so count was including it. If count was 1, now it's
            // 0 (in logic), so we delete the file.
            await _context.DeleteFileAsync<StoredFile>(userFile.FileId);
            _logger.LogInformation("Deleted physical file {FileId} as last reference was removed", userFile.FileId);
        }
        else
        {
            _logger.LogInformation("Removed reference for UserFile {UserFileId}, physical file {FileId} remains", userFileId, userFile.FileId);
        }
    }
}