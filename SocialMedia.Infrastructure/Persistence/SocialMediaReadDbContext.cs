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
            entity.ToTable("PostReads"); // Separate table or view usually

            entity.OwnsOne(p => p.Stats, b => b.ToJson());
            entity.OwnsMany(p => p.Reactions, b => b.ToJson());
            entity.OwnsMany(p => p.AdminTags, b => b.ToJson());
            entity.OwnsMany(p => p.Tags, b => b.ToJson());
            entity.OwnsMany(p => p.TopComments, b =>
            {
                b.ToJson();
                b.OwnsMany(c => c.Reactions);
                b.OwnsMany(c => c.Tags);
                b.OwnsMany(c => c.AdminTags);
            });
        });

        modelBuilder.Entity<CommentReadModel>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.ToTable("CommentReads");

            entity.OwnsOne(c => c.Stats, b => b.ToJson());
            entity.OwnsMany(c => c.Reactions, b => b.ToJson());
            entity.OwnsMany(c => c.Tags, b => b.ToJson());
            entity.OwnsMany(c => c.AdminTags, b => b.ToJson());
        });
    }
}