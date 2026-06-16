using Microsoft.Extensions.Caching.Distributed;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using StackExchange.Redis;
using SocialMedia.Application;

namespace SocialMedia.IntegrationTests;

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

            // Mock PostVectorService to avoid persistent file side effects and ONNX dependency in tests
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

            // Replace IConnectionMultiplexer with a mock to avoid Garnet dependency
            var redisDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IConnectionMultiplexer));
            if (redisDescriptor != null)
            {
                services.Remove(redisDescriptor);
            }

            // Replace IDistributedCache with in-memory cache for tests
            var cacheDescriptor = services.SingleOrDefault(d => d.ServiceType == typeof(IDistributedCache));
            if (cacheDescriptor != null)
            {
                services.Remove(cacheDescriptor);
            }
            services.AddDistributedMemoryCache();

            // Remove real ISocialTokenVerifier registrations and replace with fakes
            var verifierDescriptors = services.Where(d => d.ServiceType == typeof(ISocialTokenVerifier)).ToList();
            foreach (var d in verifierDescriptors)
            {
                services.Remove(d);
            }

            services.AddScoped<ISocialTokenVerifier, FakeFacebookTokenVerifier>();
            services.AddScoped<ISocialTokenVerifier, FakeTwitterTokenVerifier>();
            services.AddScoped<ISocialTokenVerifier, FakeGitHubTokenVerifier>();
            services.AddScoped<ISocialTokenVerifier, FakeAppleTokenVerifier>();
            services.AddScoped<ISocialTokenVerifier, FakeGoogleTokenVerifier>();
        });
    }

    private class FakeFacebookTokenVerifier : ISocialTokenVerifier
    {
        public SocialProvider Provider => SocialProvider.Facebook;
        public Task<ExternalUserInfo> VerifyAsync(string accessToken, CancellationToken cancellationToken = default)
        {
            if (accessToken == "invalid_token") throw new Exception("Invalid Facebook token");
            return Task.FromResult(new ExternalUserInfo("fbuser@example.com", "Facebook", "User", "fb_12345", SocialProvider.Facebook));
        }
    }

    private class FakeTwitterTokenVerifier : ISocialTokenVerifier
    {
        public SocialProvider Provider => SocialProvider.Twitter;
        public Task<ExternalUserInfo> VerifyAsync(string accessToken, CancellationToken cancellationToken = default)
        {
            if (accessToken == "invalid_token") throw new Exception("Invalid Twitter token");
            return Task.FromResult(new ExternalUserInfo("twuser@twitter.social-app.internal", "Twitter", "User", "tw_12345", SocialProvider.Twitter));
        }
    }

    private class FakeGitHubTokenVerifier : ISocialTokenVerifier
    {
        public SocialProvider Provider => SocialProvider.GitHub;
        public Task<ExternalUserInfo> VerifyAsync(string accessToken, CancellationToken cancellationToken = default)
        {
            if (accessToken == "invalid_token") throw new Exception("Invalid GitHub token");
            return Task.FromResult(new ExternalUserInfo("ghuser@example.com", "GitHub", "User", "gh_12345", SocialProvider.GitHub));
        }
    }

    private class FakeAppleTokenVerifier : ISocialTokenVerifier
    {
        public SocialProvider Provider => SocialProvider.Apple;
        public Task<ExternalUserInfo> VerifyAsync(string accessToken, CancellationToken cancellationToken = default)
        {
            if (accessToken == "invalid_token") throw new Exception("Invalid Apple token");
            return Task.FromResult(new ExternalUserInfo("appleuser@example.com", "Apple", "User", "apple_12345", SocialProvider.Apple));
        }
    }

    private class FakeGoogleTokenVerifier : ISocialTokenVerifier
    {
        public SocialProvider Provider => SocialProvider.Google;
        public Task<ExternalUserInfo> VerifyAsync(string accessToken, CancellationToken cancellationToken = default)
        {
            if (accessToken == "invalid_token") throw new Exception("Invalid Google token");
            return Task.FromResult(new ExternalUserInfo("googleuser@example.com", "Google", "User", "google_12345", SocialProvider.Google));
        }
    }
}