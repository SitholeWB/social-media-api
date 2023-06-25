using Microsoft.EntityFrameworkCore;
using SocialMediaApi.Data;
using SocialMediaApi.Domain.Entities;
using SocialMediaApi.Domain.Entities.Base;
using SocialMediaApi.Domain.Entities.JsonEntities;
using SocialMediaApi.Domain.Enums;

namespace SocialMediaApi.Logic.Tests
{
    public static class DbContextHelper
    {
        public static Guid _userId = Guid.Parse("8a429a96-d8dc-4af4-b8f5-30a5082017fd");

        public static BaseUser _creator = new()
        {
            Id = _userId,
            Name = "Jobe Mondise",
            ImageUrl = "https://maphitha.net/nice-image/1",
        };

        public static Guid[] _groups = new Guid[]
            {
            Guid.Parse("4839d58f-76d5-4d27-9bd4-d9618747f541"),
            Guid.Parse("415989cc-06c8-4c38-95b6-d0e716ba42d9"),
            Guid.Parse("d2f174bf-d970-43c3-8d23-91ef008682cd")
            };

        public static Guid[] _groupOnePosts = new Guid[]
        {
            Guid.Parse("0f72c01b-d46b-4fd6-b691-0a06e379ec70"),
            Guid.Parse("e8027dd8-10c0-479a-ad82-1cfd6e0c94dd"),
            Guid.Parse("771afc9c-4a49-41bf-8fe2-d7804e269cdf")
        };

        public static Guid[] _groupTwoPosts = new Guid[]
        {
            Guid.Parse("a501c634-ca9c-4dd5-bb7b-9d760dbdddbd"),
            Guid.Parse("515de86c-b487-46e2-9de7-629ee0d4d90e"),
            Guid.Parse("59a8874c-901f-41f8-98de-46282538c2c8")
        };

        public static Guid[] _groupThreePosts = new Guid[]
        {
            Guid.Parse("1b4e002c-013c-496d-af3b-5c1d51a4361c"),
            Guid.Parse("59223c3f-ca61-493d-971c-b27af83ef785"),
            Guid.Parse("66844bf4-6245-4f2d-9c99-ef2c839b73c3")
        };

        public static SocialMediaApiDbContext GetDatabaseContext()
        {
            var options = new DbContextOptionsBuilder<SocialMediaApiDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            var databaseContext = new SocialMediaApiDbContext(options);
            return databaseContext;
        }

        public static async Task BasicSetup(this SocialMediaApiDbContext dbContext)
        {
            dbContext.Database.EnsureCreated();

            await dbContext.AddAsync(CreateGroupHelper(_groups[0], "Group One", "This is group one"));
            await dbContext.AddAsync(CreateGroupHelper(_groups[1], "Group Two", "This is group two"));
            await dbContext.AddAsync(CreateGroupHelper(_groups[2], "Group Three", "This is group three"));

            await dbContext.AddAsync(CreatePostHelper(_groups[0], _groupOnePosts[0], "Post 1 - Group One"));
            await dbContext.AddAsync(CreatePostHelper(_groups[0], _groupOnePosts[1], "Post 2 - Group One"));
            await dbContext.AddAsync(CreatePostHelper(_groups[0], _groupOnePosts[2], "Post 3 - Group One"));

            await dbContext.AddAsync(CreatePostHelper(_groups[1], _groupTwoPosts[0], "Post 1 - Group Two"));
            await dbContext.AddAsync(CreatePostHelper(_groups[1], _groupTwoPosts[1], "Post 2 - Group Two"));
            await dbContext.AddAsync(CreatePostHelper(_groups[1], _groupTwoPosts[2], "Post 3 - Group Two"));

            await dbContext.AddAsync(CreatePostHelper(_groups[2], _groupThreePosts[0], "Post 1 - Group Three"));
            await dbContext.AddAsync(CreatePostHelper(_groups[2], _groupThreePosts[1], "Post 2 - Group Three"));
            await dbContext.AddAsync(CreatePostHelper(_groups[2], _groupThreePosts[2], "Post 3 - Group Three"));

            await dbContext.SaveChangesAsync();
        }

        public static Post CreatePostHelper(Guid ownerId, Guid id, string text)
        {
            return new Post
            {
                CreatedDate = DateTimeOffset.UtcNow,
                Creator = _creator,
                Id = id,
                Text = text,
                ActionBasedDate = DateTimeOffset.UtcNow,
                Downloads = 0,
                OwnerId = ownerId,
                Rank = 0,
                Reactions = new ReactionSummary { },
                TotalComments = 0,
                Views = 0,
                Media = new Media
                {
                    Content = new List<MediaContent> { },
                    MediaType = PostMediaType.TextOnly
                },
                EntityStatus = EntityStatus.Ready,
                LastModifiedDate = DateTimeOffset.UtcNow,
            };
        }

        public static Group CreateGroupHelper(Guid id, string name, string description)
        {
            return new Group
            {
                CreatedDate = DateTimeOffset.UtcNow,
                Creator = _creator,
                Id = id,
                Name = name,
                Description = description,
                EntityStatus = EntityStatus.Ready,
                LastModifiedDate = DateTimeOffset.UtcNow,
            };
        }
    }
}