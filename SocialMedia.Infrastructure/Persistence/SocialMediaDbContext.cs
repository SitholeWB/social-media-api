namespace SocialMedia.Infrastructure;

public class SocialMediaDbContext : DbContext
{
    private readonly ITenantProvider? _tenantProvider;
    public Guid CurrentTenantId { get; set; }

    public SocialMediaDbContext(DbContextOptions<SocialMediaDbContext> options, ITenantProvider? tenantProvider = null) : base(options)
    {
        _tenantProvider = tenantProvider;
        CurrentTenantId = _tenantProvider?.GetTenantId() ?? Guid.Empty;
    }

    public SocialMediaDbContext()
    {
    }

    public DbSet<Post> Posts { get; set; }
    public DbSet<Comment> Comments { get; set; }
    public DbSet<Like> Reactions { get; set; }
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
    public DbSet<PollVoteRecord> PollVoteRecords { get; set; }
    public DbSet<UserActivity> UserActivities { get; set; }
    public DbSet<Feedback> Feedbacks { get; set; }
    public DbSet<Tenant> Tenants { get; set; }
    public DbSet<PostReadModel> PostReads { get; set; }
    public DbSet<CommentReadModel> CommentReads { get; set; }
    public DbSet<StatsRecord> StatsRecords { get; set; }

    public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
    {
        foreach (var entry in ChangeTracker.Entries<BaseEntity>().Where(e => e.State == EntityState.Added))
        {
            if (entry.Entity.TenantId == Guid.Empty && CurrentTenantId != Guid.Empty)
            {
                entry.Entity.TenantId = CurrentTenantId;
            }
        }
        return base.SaveChangesAsync(cancellationToken);
    }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
            var config = new ConfigurationBuilder()
                            .SetBasePath(Directory.GetCurrentDirectory()) // ensures it looks in the right folder
                            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                            .Build();

            // Read connection string by name
            var connectionString = config.GetConnectionString("default");
            optionsBuilder.UseSqlServer(connectionString);
        }
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
            {
                var method = typeof(SocialMediaDbContext)
                    .GetMethod(nameof(SetGlobalQueryFilter), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                    ?.MakeGenericMethod(entityType.ClrType);
                method?.Invoke(this, new object[] { modelBuilder });
            }
        }

        modelBuilder.Entity<Post>()
            .HasMany(p => p.Comments)
            .WithOne(c => c.Post)
            .HasForeignKey(c => c.PostId);

        modelBuilder.Entity<Post>()
            .HasMany(p => p.Likes)
            .WithOne(l => l.Post)
            .HasForeignKey(l => l.PostId)
            .IsRequired(false);

        modelBuilder.Entity<Post>(entity =>
        {
            entity.OwnsMany(c => c.Tags, b => b.ToJson());
            entity.OwnsMany(c => c.AdminTags, b => b.ToJson());
            entity.OwnsMany(c => c.Media, b => b.ToJson());
        });

        modelBuilder.Entity<Comment>()
            .HasMany(c => c.Likes)
            .WithOne(l => l.Comment)
            .HasForeignKey(l => l.CommentId)
            .IsRequired(false);

        modelBuilder.Entity<Comment>(entity =>
        {
            entity.OwnsMany(c => c.Tags, b => b.ToJson());
            entity.OwnsMany(c => c.AdminTags, b => b.ToJson());
            entity.OwnsMany(c => c.Media, b => b.ToJson());
        });

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
            .HasOne(p => p.Group)
            .WithMany(g => g.Posts)
            .HasForeignKey(x => x.GroupId);

        modelBuilder.Entity<GroupMember>()
            .HasOne(gm => gm.Group)
            .WithMany(g => g.Members)
            .HasForeignKey(gm => gm.GroupId);

        modelBuilder.Entity<GroupMember>()
            .HasOne(gm => gm.User)
            .WithMany()
            .HasForeignKey(gm => gm.UserId);

        modelBuilder.Entity<PollVoteRecord>()
            .HasIndex(p => new { p.PollId, p.UserId })
            .IsUnique();

        modelBuilder.Entity<UserActivity>(entity =>
        {
            entity.HasIndex(ua => ua.UserId).IsUnique();
            entity.OwnsMany(ua => ua.Reactions, b => b.ToJson());
            entity.OwnsMany(ua => ua.Votes, b => b.ToJson());
        });

        //READ MODELS
        modelBuilder.Entity<PostReadModel>().HasQueryFilter(e => CurrentTenantId == Guid.Empty || e.TenantId == CurrentTenantId);
        modelBuilder.Entity<CommentReadModel>().HasQueryFilter(e => CurrentTenantId == Guid.Empty || e.TenantId == CurrentTenantId);

        foreach (var entityType in modelBuilder.Model.GetEntityTypes())
        {
            if (typeof(BaseEntity).IsAssignableFrom(entityType.ClrType))
            {
                var method = typeof(SocialMediaDbContext)
                    .GetMethod(nameof(SetGlobalQueryFilter), System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance)
                    ?.MakeGenericMethod(entityType.ClrType);
                method?.Invoke(this, new object[] { modelBuilder });
            }
        }

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

    private void SetGlobalQueryFilter<T>(ModelBuilder builder) where T : BaseEntity
    {
        builder.Entity<T>().HasQueryFilter(e => CurrentTenantId == Guid.Empty || e.TenantId == CurrentTenantId);
    }
}