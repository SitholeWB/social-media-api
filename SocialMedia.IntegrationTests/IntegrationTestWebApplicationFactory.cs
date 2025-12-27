using Moq;

namespace SocialMedia.IntegrationTests
{
    public class IntegrationTestWebApplicationFactory : WebApplicationFactory<Program>
    {
        private readonly string _dbName = Guid.NewGuid().ToString();

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
            builder.UseEnvironment("Testing");
            builder.ConfigureServices(services =>
            {
                // Remove and replace SocialMediaDbContext
                var descriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<SocialMediaDbContext>));

                if (descriptor != null)
                {
                    services.Remove(descriptor);
                }

                services.AddDbContext<SocialMediaDbContext>(options =>
                {
                    options.UseInMemoryDatabase(_dbName);
                });

                // Remove and replace SocialMediaReadDbContext
                var readDescriptor = services.SingleOrDefault(
                    d => d.ServiceType == typeof(DbContextOptions<SocialMediaReadDbContext>));

                if (readDescriptor != null)
                {
                    services.Remove(readDescriptor);
                }

                services.AddDbContext<SocialMediaReadDbContext>(options =>
                {
                    options.UseInMemoryDatabase($"{_dbName}_Read");
                });

                // Mock PostVectorService to avoid persistent file side effects and ONNX dependency
                // in tests
                var mockVectorService = new Mock<IPostVectorService>();
                services.AddSingleton(mockVectorService.Object);

                // Also mock IEmbeddingGenerator just in case
                var mockEmbeddingGenerator = new Mock<IEmbeddingGenerator>();
                services.AddSingleton(mockEmbeddingGenerator.Object);

                // Remove real SqliteVectorStore registration if present
                var vectorStoreDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(SqliteVectorStore));
                if (vectorStoreDescriptor != null)
                {
                    services.Remove(vectorStoreDescriptor);
                }
            });
        }
    }
}