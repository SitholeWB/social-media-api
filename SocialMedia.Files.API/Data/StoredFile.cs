using Files.EntityFrameworkCore.Extensions;

namespace SocialMedia.Files.API.Data;

public class StoredFile : IFileEntity
{
    public Guid Id { get; set; }
    public Guid FileId { get; set; }
    public string Name { get; set; } = string.Empty;
    public string MimeType { get; set; } = string.Empty;
    public DateTimeOffset TimeStamp { get; set; }
    public Guid? NextId { get; set; }
    public int ChunkBytesLength { get; set; }
    public long TotalBytesLength { get; set; }
    public byte[] Data { get; set; } = Array.Empty<byte>();

}
