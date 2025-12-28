using Microsoft.Extensions.DependencyInjection.Extensions;

namespace SocialMedia.Files.API.IntegrationTests;

public class IntegrationTestFactory : WebApplicationFactory<Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.UseEnvironment("Testing");
        builder.ConfigureServices(services =>
        {
            // Remove existing configurations using RemoveAll which is safer
            services.RemoveAll(typeof(DbContextOptions<FileDbContext>));
            services.RemoveAll(typeof(DbContextOptions));
            services.RemoveAll(typeof(FileDbContext));

            // Add InMemory DbContext (Generic, ignoring sharding for basic tests or simulating it)
            // But Program.cs has lambda registration. If we remove it, we need to replace it with
            // something that mimics the behavior OR just a simple one if we don't care about
            // sharding tests in this specific factory.

            // For Integration Tests, we usually want to test the full stack properly. But InMemory
            // doesn't fully support all Relational features.

            // Let's replace with InMemory but using the SAME lambda signature if possible? No,
            // easiest is to just register it simply and assume 'shardKey' is ignored by InMemory
            // provider or we manage it.

            services.AddDbContext<FileDbContext>(options =>
            {
                options.UseInMemoryDatabase("TestDb");
            });

            // Note: This replaces the "Per Request" configuration in Program.cs with a static configuration.
            // This means Sharding Logic in Program.cs is BYPASSED in tests. This is acceptable for
            // functional testing of Controller Logic (Upload/Download). To test Sharding
            // specifically, we would need a more complex setup.

            // Add Newtonsoft.Json for testing
            services.AddControllers()
                .AddNewtonsoftJson();
        });
    }
}