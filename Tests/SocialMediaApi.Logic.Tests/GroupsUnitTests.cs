using SocialMediaApi.Data;

namespace SocialMediaApi.Logic.Tests
{
    public class Tests
    {
        private SocialMediaApiDbContext _context;

        [SetUp]
        public async Task Setup()
        {
            _context = DbContextHelper.GetDatabaseContext();
            await _context.BasicSetup();
        }

        [Test]
        public async Task Test1()
        {
            Assert.Pass();
        }
    }
}