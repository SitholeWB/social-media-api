using SocialMedia.Domain;

namespace SocialMedia.Infrastructure;

public class SocialMediaReadDbContext : DbContext
{
    public SocialMediaReadDbContext(DbContextOptions<SocialMediaReadDbContext> options) : base(options)
    {
    }

    public SocialMediaReadDbContext()
    {
    }

    public DbSet<PostReadModel> Posts { get; set; }
    public DbSet<CommentReadModel> Comments { get; set; }
    public DbSet<StatsRecord> StatsRecords { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            var config = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory()) // ensures it looks in the right folder
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();
            // Read connection string by name
            var connectionString = config.GetConnectionString("ReadConnection");
            optionsBuilder.UseSqlServer(connectionString);
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<PostReadModel>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.GroupId);
            entity.ToTable("PostReads"); // Separate table or view usually

            entity.OwnsMany(p => p.Reactions, b => b.ToJson());
            entity.OwnsMany(p => p.AdminTags, b => b.ToJson());
            entity.OwnsMany(p => p.Tags, b => b.ToJson());
            entity.OwnsMany(p => p.Media, b => b.ToJson());
            entity.OwnsMany(p => p.TopComments, b =>
            {
                b.ToJson();
                b.OwnsMany(c => c.Reactions);
                b.OwnsMany(c => c.Tags);
                b.OwnsMany(c => c.AdminTags);
                b.OwnsMany(c => c.Media);
            });

            // Index for ranking queries
            entity.HasIndex(p => new { p.TrendingScore, p.CreatedAt })
                  .IsDescending(true, true)
                  .HasDatabaseName("IX_Posts_RankScore_CreatedAt");

            entity.HasIndex(p => p.CreatedAt)
                  .IsDescending()
                  .HasDatabaseName("IX_Posts_CreatedAt");

            entity.HasIndex(p => p.ReactionCount)
                  .HasDatabaseName("IX_Posts_ReactionCount");

            entity.HasIndex(p => p.CommentCount)
                  .HasDatabaseName("IX_Posts_CommentCount");
        });

        modelBuilder.Entity<CommentReadModel>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasIndex(e => e.PostId);
            entity.ToTable("CommentReads");

            entity.OwnsOne(c => c.Stats, b => b.ToJson());
            entity.OwnsMany(c => c.Reactions, b => b.ToJson());
            entity.OwnsMany(c => c.Tags, b => b.ToJson());
            entity.OwnsMany(c => c.AdminTags, b => b.ToJson());
            entity.OwnsMany(c => c.Media, b => b.ToJson());
        });

        modelBuilder.Entity<StatsRecord>(entity =>
        {
            entity.OwnsMany(s => s.ReactionBreakdown, b => b.ToJson());
        });
    }
}