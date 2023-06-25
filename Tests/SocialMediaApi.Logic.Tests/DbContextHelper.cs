using Microsoft.EntityFrameworkCore;
using SocialMediaApi.Data;

namespace SocialMediaApi.Logic.Tests
{
    public class DbContextHelper
    {
        public static SocialMediaApiDbContext GetDatabaseContext()
        {
            var options = new DbContextOptionsBuilder<SocialMediaApiDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            var databaseContext = new SocialMediaApiDbContext(options);
            return databaseContext;
        }
    }
}