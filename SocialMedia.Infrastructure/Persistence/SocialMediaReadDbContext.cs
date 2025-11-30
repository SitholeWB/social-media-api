using Microsoft.EntityFrameworkCore;
using SocialMedia.Domain.ReadModels;

namespace SocialMedia.Infrastructure.Persistence;

public class SocialMediaReadDbContext : DbContext
{
    public SocialMediaReadDbContext(DbContextOptions<SocialMediaReadDbContext> options) : base(options)
    {
    }

    public DbSet<PostReadModel> Posts { get; set; }
    public DbSet<CommentReadModel> Comments { get; set; }

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
    }
}
