using Microsoft.EntityFrameworkCore;

namespace SocialMedia.Files.API;

public class FileDbContext : DbContext
{
    public DbSet<UserFile> UserFiles { get; set; }

    public FileDbContext(DbContextOptions<FileDbContext> options) : base(options)
    {
    }

    public DbSet<StoredFile> Files { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            var config = new ConfigurationBuilder()
                            .SetBasePath(Directory.GetCurrentDirectory()) // ensures it looks in the right folder
                            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                            .Build();

            // Read connection string by name
            var connectionString = config.GetConnectionString("db1");
            optionsBuilder.UseSqlServer(connectionString);
        }
    }
}