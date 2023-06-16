using Microsoft.EntityFrameworkCore;
using SocialMediaApi.Domain.Entities;

namespace SocialMediaApi.Data
{
    public class SocialMediaApiDbContext : DbContext
    {
        public SocialMediaApiDbContext(DbContextOptions<SocialMediaApiDbContext> options) : base(options)
        {
            Database.Migrate();
        }

        public DbSet<Group> Groups { get; set; } = default!;
        public DbSet<ActivePost> ActivePosts { get; set; } = default!;
        public DbSet<Post> Posts { get; set; } = default!;
        public DbSet<PostComment> PostComments { get; set; } = default!;
        public DbSet<EntityDetails> EntityDetails { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Group>().OwnsOne(
                group => group.Creator, ownedNavigationBuilder =>
                {
                    ownedNavigationBuilder.ToJson();
                });
            modelBuilder.Entity<Group>().HasMany(x => x.Posts).WithOne(x => x.Group).HasForeignKey(x => x.GroupId);
            modelBuilder.Entity<Post>().HasMany(x => x.PostComments).WithOne(x => x.Post).HasForeignKey(x => x.PostId);
            modelBuilder.Entity<Group>().HasIndex(x => x.EntityStatus);
            modelBuilder.Entity<Post>().HasIndex(x => x.ActionBasedDate);
            modelBuilder.Entity<ActivePost>().HasIndex(x => x.ActionBasedDate);
            modelBuilder.Entity<ActivePost>().HasIndex(x => x.GroupId);
            //Post
            modelBuilder.Entity<Post>().OwnsOne(
                post => post.Creator, ownedNavigationBuilder =>
                {
                    ownedNavigationBuilder.ToJson();
                });
            modelBuilder.Entity<Post>().OwnsOne(
                post => post.Media, ownedNavigationBuilder =>
                {
                    ownedNavigationBuilder.ToJson();
                    ownedNavigationBuilder.OwnsMany(media => media.Content);
                });
            modelBuilder.Entity<Post>().OwnsOne(
                post => post.Reactions, ownedNavigationBuilder =>
                {
                    ownedNavigationBuilder.ToJson();
                    ownedNavigationBuilder.OwnsMany(x => x.Emojis);
                });

            //ActivePost
            modelBuilder.Entity<ActivePost>().OwnsOne(
                post => post.Creator, ownedNavigationBuilder =>
                {
                    ownedNavigationBuilder.ToJson();
                });
            modelBuilder.Entity<ActivePost>().OwnsOne(
                post => post.Media, ownedNavigationBuilder =>
                {
                    ownedNavigationBuilder.ToJson();
                    ownedNavigationBuilder.OwnsMany(media => media.Content);
                });
            modelBuilder.Entity<ActivePost>().OwnsOne(
                post => post.Reactions, ownedNavigationBuilder =>
                {
                    ownedNavigationBuilder.ToJson();
                    ownedNavigationBuilder.OwnsMany(x => x.Emojis);
                });

            //PostComment
            modelBuilder.Entity<PostComment>().OwnsOne(
                post => post.Creator, ownedNavigationBuilder =>
                {
                    ownedNavigationBuilder.ToJson();
                });
            modelBuilder.Entity<PostComment>().OwnsOne(
                post => post.Media, ownedNavigationBuilder =>
                {
                    ownedNavigationBuilder.ToJson();
                    ownedNavigationBuilder.OwnsMany(media => media.Content);
                });
            modelBuilder.Entity<PostComment>().OwnsOne(
                post => post.Reactions, ownedNavigationBuilder =>
                {
                    ownedNavigationBuilder.ToJson();
                    ownedNavigationBuilder.OwnsMany(x => x.Emojis);
                });

            //EntityDetails
            modelBuilder.Entity<EntityDetails>().OwnsOne(
                post => post.Summary, ownedNavigationBuilder =>
                {
                    ownedNavigationBuilder.ToJson();
                    ownedNavigationBuilder.OwnsMany(x => x.Emojis);
                });
            modelBuilder.Entity<EntityDetails>().OwnsMany(
                post => post.Reactions, ownedNavigationBuilder =>
                {
                    ownedNavigationBuilder.ToJson();
                    ownedNavigationBuilder.OwnsOne(x => x.Creator);
                });
        }
    }
}