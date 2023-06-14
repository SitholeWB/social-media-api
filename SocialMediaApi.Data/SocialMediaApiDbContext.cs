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
        public DbSet<ActiveGroupPost> ActiveGroupPosts { get; set; } = default!;
        public DbSet<GroupPost> GroupPosts { get; set; } = default!;
        public DbSet<GroupPostComment> GroupPostComments { get; set; } = default!;
        public DbSet<Reaction> Reactions { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Group>().OwnsOne(
                group => group.Creator, ownedNavigationBuilder =>
                {
                    ownedNavigationBuilder.ToJson();
                });
            modelBuilder.Entity<Group>().HasMany(x => x.Posts).WithOne(x => x.Group).HasForeignKey(x => x.GroupId);
            modelBuilder.Entity<GroupPost>().HasMany(x => x.GroupPostComments).WithOne(x => x.GroupPost).HasForeignKey(x => x.GroupPostId);
            modelBuilder.Entity<Group>().HasIndex(x => x.EntityStatus);
            modelBuilder.Entity<GroupPost>().HasIndex(x => x.ActionBasedDate);
            modelBuilder.Entity<ActiveGroupPost>().HasIndex(x => x.ActionBasedDate);
            modelBuilder.Entity<ActiveGroupPost>().HasIndex(x => x.GroupId);
            modelBuilder.Entity<Reaction>().HasIndex(x => x.OwnerId);
            //GroupPost
            modelBuilder.Entity<GroupPost>().OwnsOne(
                groupPost => groupPost.Creator, ownedNavigationBuilder =>
                {
                    ownedNavigationBuilder.ToJson();
                });
            modelBuilder.Entity<GroupPost>().OwnsOne(
                groupPost => groupPost.Media, ownedNavigationBuilder =>
                {
                    ownedNavigationBuilder.ToJson();
                    ownedNavigationBuilder.OwnsMany(media => media.Content);
                });
            modelBuilder.Entity<GroupPost>().OwnsOne(
                groupPost => groupPost.Reactions, ownedNavigationBuilder =>
                {
                    ownedNavigationBuilder.ToJson();
                    ownedNavigationBuilder.OwnsMany(x => x.Emojis);
                });

            //ActiveGroupPost
            modelBuilder.Entity<ActiveGroupPost>().OwnsOne(
                groupPost => groupPost.Creator, ownedNavigationBuilder =>
                {
                    ownedNavigationBuilder.ToJson();
                });
            modelBuilder.Entity<ActiveGroupPost>().OwnsOne(
                groupPost => groupPost.Media, ownedNavigationBuilder =>
                {
                    ownedNavigationBuilder.ToJson();
                    ownedNavigationBuilder.OwnsMany(media => media.Content);
                });
            modelBuilder.Entity<ActiveGroupPost>().OwnsOne(
                groupPost => groupPost.Reactions, ownedNavigationBuilder =>
                {
                    ownedNavigationBuilder.ToJson();
                    ownedNavigationBuilder.OwnsMany(x => x.Emojis);
                });

            //GroupPostComment
            modelBuilder.Entity<GroupPostComment>().OwnsOne(
                groupPost => groupPost.Creator, ownedNavigationBuilder =>
                {
                    ownedNavigationBuilder.ToJson();
                });
            modelBuilder.Entity<GroupPostComment>().OwnsOne(
                groupPost => groupPost.Media, ownedNavigationBuilder =>
                {
                    ownedNavigationBuilder.ToJson();
                    ownedNavigationBuilder.OwnsMany(media => media.Content);
                });
            modelBuilder.Entity<GroupPostComment>().OwnsOne(
                groupPost => groupPost.Reactions, ownedNavigationBuilder =>
                {
                    ownedNavigationBuilder.ToJson();
                    ownedNavigationBuilder.OwnsMany(x => x.Emojis);
                });

            //GroupPostComment
            modelBuilder.Entity<Reaction>().OwnsOne(
                groupPost => groupPost.Creator, ownedNavigationBuilder =>
                {
                    ownedNavigationBuilder.ToJson();
                });
        }
    }
}