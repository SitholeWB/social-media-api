using Microsoft.Extensions.DependencyInjection;
using SocialMedia.Files.API.Data;
using System.Net;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using Xunit;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace SocialMedia.Files.API.IntegrationTests;

public class FilesControllerTests : IClassFixture<IntegrationTestFactory>
{
    private readonly IntegrationTestFactory _factory;
    private readonly HttpClient _client;

    public FilesControllerTests(IntegrationTestFactory factory)
    {
        _factory = factory;
        _client = _factory.CreateClient();
    }

    private string GenerateToken(string secret)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(issuer: "test", audience: "test", claims: new[] { new Claim("sub", "user1") }, expires: DateTime.Now.AddMinutes(10), signingCredentials: credentials);
        return new JwtSecurityTokenHandler().WriteToken(token);
    }
    
    // Note: appsettings.json in Program.cs has "super_secret_key_1", "super_secret_key_2".
    // We should use those or override config.
    // Assuming appsettings.json is loaded in test.

    [Fact]
    public async Task Upload_WithValidAuth_ShouldSucceed()
    {
        var token = GenerateToken("this_is_a_super_secret_key_for_testing_purposes_1");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        
        var content = new MultipartFormDataContent();
        content.Add(new ByteArrayContent(Encoding.UTF8.GetBytes("Hello World")), "file", "hello.txt");
        
        var response = await _client.PostAsync("/api/db1/files", content);
        
        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var json = await response.Content.ReadAsStringAsync();
        Assert.Contains("id", json);
    }
    
    [Fact]
    public async Task Get_Public_ShouldSucceed()
    {
        // 1. Upload first (needs auth)
        var token = GenerateToken("this_is_a_super_secret_key_for_testing_purposes_1");
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        
        var content = new MultipartFormDataContent();
        content.Add(new ByteArrayContent(Encoding.UTF8.GetBytes("Public Content")), "file", "public.txt");
        var upResponse = await _client.PostAsync("/api/db1/files", content);
        upResponse.EnsureSuccessStatusCode();
        
        var json = JsonDocument.Parse(await upResponse.Content.ReadAsStringAsync());
        var id = json.RootElement.GetProperty("id").GetString();
        
        // 2. Download (no auth)
        _client.DefaultRequestHeaders.Authorization = null;
        var downResponse = await _client.GetAsync($"/api/db1/files/{id}");
        
        Assert.Equal(HttpStatusCode.OK, downResponse.StatusCode);
        Assert.Equal("Public Content", await downResponse.Content.ReadAsStringAsync());
    }
    
    [Fact]
    public async Task Delete_ShouldDelete()
    {
         // 1. Upload
        var token = GenerateToken("this_is_a_super_secret_key_for_testing_purposes_2"); // Use second key
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        
        var content = new MultipartFormDataContent();
        content.Add(new ByteArrayContent(Encoding.UTF8.GetBytes("Delete Me")), "file", "del.txt");
        var upResponse = await _client.PostAsync("/api/db1/files", content);
        upResponse.EnsureSuccessStatusCode();
        
        var json = JsonDocument.Parse(await upResponse.Content.ReadAsStringAsync());
        var id = json.RootElement.GetProperty("id").GetString();
        
        // 2. Delete
        var delResponse = await _client.DeleteAsync($"/api/db1/files/{id}");
        Assert.Equal(HttpStatusCode.NoContent, delResponse.StatusCode);
        
        // 3. Verify Gone
        var getResponse = await _client.GetAsync($"/api/db1/files/{id}");
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }
}
