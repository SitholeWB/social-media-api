namespace SocialMedia.Infrastructure;

public class SocialMediaDbContext : DbContext
{
    public SocialMediaDbContext(DbContextOptions<SocialMediaDbContext> options) : base(options)
    {
    }

    public SocialMediaDbContext()
    {
    }

    public DbSet<Post> Posts { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<Like> Likes { get; set; }
    public DbSet<MediaFile> MediaFiles { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<Poll> Polls { get; set; }
    public DbSet<PollOption> PollOptions { get; set; }
    public DbSet<Vote> Votes { get; set; }
    public DbSet<Block> Blocks { get; set; }
    public DbSet<Report> Reports { get; set; }
    public DbSet<UserBlock> UserBlocks { get; set; }
    public DbSet<Notification> Notifications { get; set; }
    public DbSet<Group> Groups { get; set; }
    public DbSet<GroupMember> GroupMembers { get; set; }
    public DbSet<OutboxEvent> OutboxEvents { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            var config = new ConfigurationBuilder()
                            .SetBasePath(AppContext.BaseDirectory) // ensures it looks in the right folder
                            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                            .Build();

            // Read connection string by name
            var connectionString = config.GetConnectionString("WriteConnection");
            optionsBuilder.UseSqlServer(connectionString);
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Post>()
            .HasMany(p => p.Comments)
            .WithOne(c => c.Post)
            .HasForeignKey(c => c.PostId);

        modelBuilder.Entity<Post>()
            .HasMany(p => p.Likes)
            .WithOne(l => l.Post)
            .HasForeignKey(l => l.PostId)
            .IsRequired(false);

        modelBuilder.Entity<Comment>()
            .HasMany(c => c.Likes)
            .WithOne(l => l.Comment)
            .HasForeignKey(l => l.CommentId)
            .IsRequired(false);

        modelBuilder.Entity<Comment>()
            .Property(p => p.Tags)
            .HasConversion(
                v => JsonSerializer.Serialize(v, new JsonSerializerOptions()), // to DB
                v => JsonSerializer.Deserialize<List<string>>(v, new JsonSerializerOptions()) ?? new List<string>() // from DB
            ).HasColumnType("nvarchar(max)");
        modelBuilder.Entity<Comment>()
            .Property(p => p.AdminTags)
            .HasConversion(
                v => JsonSerializer.Serialize(v, new JsonSerializerOptions()), // to DB
                v => JsonSerializer.Deserialize<List<string>>(v, new JsonSerializerOptions()) ?? new List<string>() // from DB
            ).HasColumnType("nvarchar(max)");

        modelBuilder.Entity<Post>()
            .Property(p => p.Tags)
            .HasConversion(
                v => JsonSerializer.Serialize(v, new JsonSerializerOptions()), // to DB
                v => JsonSerializer.Deserialize<List<string>>(v, new JsonSerializerOptions()) ?? new List<string>() // from DB
            ).HasColumnType("nvarchar(max)");
        modelBuilder.Entity<Post>()
            .Property(p => p.AdminTags)
            .HasConversion(
                v => JsonSerializer.Serialize(v, new JsonSerializerOptions()), // to DB
                v => JsonSerializer.Deserialize<List<string>>(v, new JsonSerializerOptions()) ?? new List<string>() // from DB
            ).HasColumnType("nvarchar(max)");

        modelBuilder.Entity<Poll>()
            .HasMany(p => p.Options)
            .WithOne(o => o.Poll)
            .HasForeignKey(o => o.PollId);

        modelBuilder.Entity<PollOption>()
            .HasMany(o => o.Votes)
            .WithOne(v => v.PollOption)
            .HasForeignKey(v => v.PollOptionId);

        modelBuilder.Entity<Report>()
            .HasOne(r => r.Post)
            .WithMany()
            .HasForeignKey(r => r.PostId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<Report>()
            .HasOne(r => r.Comment)
            .WithMany()
            .HasForeignKey(r => r.CommentId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<UserBlock>()
            .HasOne(ub => ub.Blocker)
            .WithMany()
            .HasForeignKey(ub => ub.BlockerId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<UserBlock>()
            .HasOne(ub => ub.BlockedUser)
            .WithMany()
            .HasForeignKey(ub => ub.BlockedUserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Post>()
            .HasMany(p => p.Groups)
            .WithMany(g => g.Posts);

        modelBuilder.Entity<GroupMember>()
            .HasOne(gm => gm.Group)
            .WithMany(g => g.Members)
            .HasForeignKey(gm => gm.GroupId);

        modelBuilder.Entity<GroupMember>()
            .HasOne(gm => gm.User)
            .WithMany()
            .HasForeignKey(gm => gm.UserId);
    }
}