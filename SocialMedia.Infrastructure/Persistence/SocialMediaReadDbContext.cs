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
                .SetBasePath(AppContext.BaseDirectory) // ensures it looks in the right folder
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
            entity.OwnsMany(p => p.TopComments, b =>
            {
                b.ToJson();
                b.OwnsMany(c => c.Reactions);
            });
        });

        modelBuilder.Entity<CommentReadModel>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.ToTable("CommentReads");

            entity.OwnsOne(c => c.Stats, b => b.ToJson());
            entity.OwnsMany(c => c.Reactions, b => b.ToJson());
        });

        modelBuilder.Entity<CommentReadModel>()
            .Property(p => p.Tags)
            .HasConversion(
                v => JsonSerializer.Serialize(v, new JsonSerializerOptions()), // to DB
                v => JsonSerializer.Deserialize<List<string>>(v, new JsonSerializerOptions()) ?? new List<string>() // from DB
            ).HasColumnType("nvarchar(max)");
        modelBuilder.Entity<CommentReadModel>()
            .Property(p => p.AdminTags)
            .HasConversion(
                v => JsonSerializer.Serialize(v, new JsonSerializerOptions()), // to DB
                v => JsonSerializer.Deserialize<List<string>>(v, new JsonSerializerOptions()) ?? new List<string>() // from DB
            ).HasColumnType("nvarchar(max)");

        modelBuilder.Entity<PostReadModel>()
            .Property(p => p.Tags)
            .HasConversion(
                v => JsonSerializer.Serialize(v, new JsonSerializerOptions()), // to DB
                v => JsonSerializer.Deserialize<List<string>>(v, new JsonSerializerOptions()) ?? new List<string>() // from DB
            ).HasColumnType("nvarchar(max)");
        modelBuilder.Entity<PostReadModel>()
            .Property(p => p.AdminTags)
            .HasConversion(
                v => JsonSerializer.Serialize(v, new JsonSerializerOptions()), // to DB
                v => JsonSerializer.Deserialize<List<string>>(v, new JsonSerializerOptions()) ?? new List<string>() // from DB
            ).HasColumnType("nvarchar(max)");
    }
}