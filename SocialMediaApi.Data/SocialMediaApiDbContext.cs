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
        public DbSet<Comment> Comments { get; set; } = default!;
        public DbSet<EntityDetails> EntityDetails { get; set; } = default!;
        public DbSet<UserDetails> UserDetails { get; set; } = default!;
        public DbSet<UserPost> UserPosts { get; set; } = default!;
        public DbSet<User> Users { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Group>().OwnsOne(
                group => group.Creator, ownedNavigationBuilder =>
                {
                    ownedNavigationBuilder.ToJson();
                });
            modelBuilder.Entity<Group>().HasIndex(x => x.EntityStatus);

            modelBuilder.Entity<User>().HasIndex(x => x.Email);
            modelBuilder.Entity<User>().HasIndex(x => x.UserName);

            modelBuilder.Entity<UserPost>().HasIndex(x => x.UserId);

            modelBuilder.Entity<Post>().HasIndex(x => x.ActionBasedDate);
            modelBuilder.Entity<Post>().HasIndex(x => x.GroupId);

            modelBuilder.Entity<Comment>().HasIndex(x => x.PostId);

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

            //Comment
            modelBuilder.Entity<Comment>().OwnsOne(
                post => post.Creator, ownedNavigationBuilder =>
                {
                    ownedNavigationBuilder.ToJson();
                });
            modelBuilder.Entity<Comment>().OwnsOne(
                post => post.Media, ownedNavigationBuilder =>
                {
                    ownedNavigationBuilder.ToJson();
                    ownedNavigationBuilder.OwnsMany(media => media.Content);
                });
            modelBuilder.Entity<Comment>().OwnsOne(
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
            //UserDetails
            modelBuilder.Entity<UserDetails>().OwnsMany(
                post => post.PostReactions, ownedNavigationBuilder =>
                {
                    ownedNavigationBuilder.ToJson();
                });
            modelBuilder.Entity<UserDetails>().OwnsMany(
                post => post.CommentReactions, ownedNavigationBuilder =>
                {
                    ownedNavigationBuilder.ToJson();
                });
            //UserPosts
            modelBuilder.Entity<UserPost>().OwnsMany(
                post => post.Posts, ownedNavigationBuilder =>
                {
                    ownedNavigationBuilder.ToJson();
                });
        }
    }
}