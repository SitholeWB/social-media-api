using Microsoft.EntityFrameworkCore;

namespace SocialMedia.Files.API.Data;

public class FileDbContext : DbContext
{
    public DbSet<UserFile> UserFiles { get; set; }

    public FileDbContext(DbContextOptions<FileDbContext> options) : base(options)
    {
    }

    public DbSet<StoredFile> Files { get; set; }
}