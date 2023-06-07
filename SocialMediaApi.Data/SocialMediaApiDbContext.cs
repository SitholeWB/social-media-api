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
        public DbSet<GroupPost> GroupPosts { get; set; } = default!;
        public DbSet<ActiveGroupPost> ActiveGroupPosts { get; set; } = default!;
        public DbSet<NewGroupPost> NewGroupPosts { get; set; } = default!;

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.Entity<Group>().OwnsOne(
                group => group.Creator, ownedNavigationBuilder =>
                {
                    ownedNavigationBuilder.ToJson();
                });
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

            //NewGroupPost
            modelBuilder.Entity<NewGroupPost>().OwnsOne(
                groupPost => groupPost.Creator, ownedNavigationBuilder =>
                {
                    ownedNavigationBuilder.ToJson();
                });
            modelBuilder.Entity<NewGroupPost>().OwnsOne(
                groupPost => groupPost.Media, ownedNavigationBuilder =>
                {
                    ownedNavigationBuilder.ToJson();
                    ownedNavigationBuilder.OwnsMany(media => media.Content);
                });
            modelBuilder.Entity<NewGroupPost>().OwnsOne(
                groupPost => groupPost.Reactions, ownedNavigationBuilder =>
                {
                    ownedNavigationBuilder.ToJson();
                    ownedNavigationBuilder.OwnsMany(x => x.Emojis);
                });
        }
    }
}