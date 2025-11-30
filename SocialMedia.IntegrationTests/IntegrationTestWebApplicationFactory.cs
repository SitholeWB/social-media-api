using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using SocialMedia.Infrastructure.Persistence;

namespace SocialMedia.IntegrationTests
{
    public class IntegrationTestWebApplicationFactory : WebApplicationFactory<Program>
    {
        private readonly string _dbName = Guid.NewGuid().ToString();

        protected override void ConfigureWebHost(IWebHostBuilder builder)
        {
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
            });
        }
    }
}
