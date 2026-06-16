using System;
using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.EntityFrameworkCore;
using SocialMedia.Infrastructure;
using Xunit;

namespace SocialMedia.IntegrationTests;

public class DefaultsControllerTests(IntegrationTestWebApplicationFactory factory) : BaseControllerTests(factory)
{
    [Fact]
    public async Task InitGroups_ShouldReturnOk_WhenUserIsAdmin()
    {
        // Arrange
        var uniqueId = Guid.NewGuid().ToString("N");
        var username = $"admin_defaults_{uniqueId}@test.com";
        var (token, _) = await RegisterAndLoginAsync(username, "password123", isAdmin: true);
        
        using var request = new HttpRequestMessage(HttpMethod.Post, $"{Constants.ApiBase}/defaults/init");
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

        // Act
        var response = await _client.SendAsync(request, TestContext.Current.CancellationToken);

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        Assert.Contains("Home", content);
    }

    [Fact]
    public async Task InitTenants_ShouldReturnOk_WhenUserIsSuperAdmin()
    {
        // Arrange
        var uniqueId = Guid.NewGuid().ToString("N");
        var username = $"superadmin_defaults_{uniqueId}@test.com";
        
        // Register and login as SuperAdmin of the SuperTenant
        // For super admin, we need the SuperTenantId claim, let's see how register/login handles tenantId.
        // Wait, RegisterAndLoginAsync registers a user. Let's make sure they are superadmin.
        var (token, _) = await RegisterAndLoginAsync(username, "password123", isSuperAdmin: true);
        
        // We need to set the user's TenantId to SuperTenantId in the DB, so their JWT token (which contains tenantId claim) matches the SuperTenant.
        // Wait, RegisterAndLoginAsync registers the user and returns a token immediately.
        // Let's modify the user's TenantId in the database, and then log in again to get the token with the correct TenantId claim!
        using (var scope = _factory.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<SocialMediaDbContext>();
            var dbUser = await dbContext.Users.FirstOrDefaultAsync(u => u.Username == username, TestContext.Current.CancellationToken);
            if (dbUser != null)
            {
                dbUser.TenantId = SocialMedia.Domain.Tenant.SuperTenantId;
                dbUser.Role = Domain.UserRole.SuperAdmin;
                await dbContext.SaveChangesAsync(TestContext.Current.CancellationToken);
            }
        }

        // Log in again to get a token with the SuperTenantId claim
        var loginResponse = await _client.PostAsJsonAsync($"{Constants.ApiBase}/auth/login", new LoginRequest(username, "password123"), TestContext.Current.CancellationToken);
        loginResponse.EnsureSuccessStatusCode();
        var authResponse = await loginResponse.Content.ReadFromJsonAsync<AuthResponse>(TestContext.Current.CancellationToken);
        var superToken = authResponse!.Token;

        // Route with SuperTenantId to pass TenantValidationMiddleware mismatch check
        var url = $"/api/v1/{SocialMedia.Domain.Tenant.SuperTenantId}/defaults/tenants";
        using var request = new HttpRequestMessage(HttpMethod.Post, url);
        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", superToken);

        // Act
        var response = await _client.SendAsync(request, TestContext.Current.CancellationToken);

        // Assert
        response.EnsureSuccessStatusCode();
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var content = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        Assert.Contains("AmaZulu FC", content);
        Assert.Contains("Kaizer Chiefs FC", content);
        Assert.Contains("Orlando Pirates FC", content);
        Assert.Contains("Golden Arrows FC", content);

        // Verify tenants exist in DB
        using (var scope = _factory.Services.CreateScope())
        {
            var dbContext = scope.ServiceProvider.GetRequiredService<SocialMediaDbContext>();
            var tenants = await dbContext.Tenants.ToListAsync(TestContext.Current.CancellationToken);
            Assert.Contains(tenants, t => t.Name == "AmaZulu FC");
            Assert.Contains(tenants, t => t.Name == "Kaizer Chiefs FC");
            Assert.Contains(tenants, t => t.Name == "Orlando Pirates FC");
            Assert.Contains(tenants, t => t.Name == "Golden Arrows FC");
        }
    }
}
