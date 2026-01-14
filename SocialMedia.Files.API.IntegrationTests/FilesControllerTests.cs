using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Text.Json;

namespace SocialMedia.Files.API.IntegrationTests;

public class FilesControllerTests : IClassFixture<IntegrationTestFactory>
{
    private readonly IntegrationTestFactory _factory;
    private readonly HttpClient _client;
    private static readonly Guid _userId = Guid.Parse("11111111-1111-1111-1111-111111111111");

    public FilesControllerTests(IntegrationTestFactory factory)
    {
        _factory = factory;
        _client = _factory.CreateDefaultClient();
    }

    private static string GenerateToken(string secret, Guid userId)
    {
        var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(secret));
        var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
        var token = new JwtSecurityToken(issuer: "test", audience: "test", claims: [new Claim(ClaimTypes.NameIdentifier, userId.ToString())], expires: DateTime.Now.AddHours(1), signingCredentials: credentials);
        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    // Note: appsettings.json in Program.cs has "super_secret_key_1", "super_secret_key_2". We
    // should use those or override config. Assuming appsettings.json is loaded in test.

    [Fact]
    public async Task Upload_WithValidAuth_ShouldSucceed()
    {
        var token = GenerateToken("this_is_a_super_secret_key_for_testing_purposes_1", _userId);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var content = new MultipartFormDataContent();
        content.Add(new ByteArrayContent(Encoding.UTF8.GetBytes("Hello World")), "file", "hello.txt");

        var response = await _client.PostAsync("/api/db1/files", content, TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var json = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        Assert.Contains("id", json);
    }

    [Fact]
    public async Task Upload_WithCustomFileId_ShouldSucceed_WithCustomId()
    {
        var token = GenerateToken("this_is_a_super_secret_key_for_testing_purposes_1", _userId);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var content = new MultipartFormDataContent();
        content.Add(new ByteArrayContent(Encoding.UTF8.GetBytes("Hello World")), "file", "hello.txt");

        var response = await _client.PostAsync($"/api/db1/files?fileId={_userId}", content, TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var json = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        Assert.Contains("id", json);
        Assert.Contains(_userId.ToString(), json);
    }

    [Fact]
    public async Task Upload_WithCustomFileId_ShouldFail_WhenUserDoNotOwnFile()
    {
        var token = GenerateToken("this_is_a_super_secret_key_for_testing_purposes_1", _userId);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var content = new MultipartFormDataContent();
        content.Add(new ByteArrayContent(Encoding.UTF8.GetBytes("Hello World")), "file", "hello.txt");

        var response = await _client.PostAsync($"/api/db1/files?fileId={_userId}", content, TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.OK, response.StatusCode);
        var json = await response.Content.ReadAsStringAsync(TestContext.Current.CancellationToken);
        Assert.Contains("id", json);
        Assert.Contains(_userId.ToString(), json);

        token = GenerateToken("this_is_a_super_secret_key_for_testing_purposes_1", Guid.NewGuid());
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        response = await _client.PostAsync($"/api/db1/files?fileId={_userId}", content, TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
    }

    [Fact]
    public async Task Get_Public_ShouldSucceed()
    {
        // 1. Upload first (needs auth)
        var token = GenerateToken("this_is_a_super_secret_key_for_testing_purposes_1", _userId);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var content = new MultipartFormDataContent();
        content.Add(new ByteArrayContent(Encoding.UTF8.GetBytes("Public Content")), "file", "public.txt");
        var upResponse = await _client.PostAsync("/api/db1/files", content, TestContext.Current.CancellationToken);
        upResponse.EnsureSuccessStatusCode();

        var json = JsonDocument.Parse(await upResponse.Content.ReadAsStringAsync(TestContext.Current.CancellationToken));
        var id = json.RootElement.GetProperty("id").GetString();

        // 2. Download (no auth)
        _client.DefaultRequestHeaders.Authorization = null;
        var downResponse = await _client.GetAsync($"/api/db1/files/{id}", TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.OK, downResponse.StatusCode);
        Assert.Equal("Public Content", await downResponse.Content.ReadAsStringAsync(TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task Get_PublicWithCustomFileId_ShouldSucceed_EvenWithFileUpdate()
    {
        // 1. Upload first (needs auth)
        var token = GenerateToken("this_is_a_super_secret_key_for_testing_purposes_1", _userId);
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var content = new MultipartFormDataContent();
        content.Add(new ByteArrayContent(Encoding.UTF8.GetBytes("Public Content")), "file", "public.txt");
        var upResponse = await _client.PostAsync($"/api/db1/files?fileId={_userId}", content, TestContext.Current.CancellationToken);
        upResponse.EnsureSuccessStatusCode();

        // 2. Download (no auth)
        _client.DefaultRequestHeaders.Authorization = null;
        var downResponse = await _client.GetAsync($"/api/db1/files/{_userId}", TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.OK, downResponse.StatusCode);
        Assert.Equal("Public Content", await downResponse.Content.ReadAsStringAsync(TestContext.Current.CancellationToken));

        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        content = new MultipartFormDataContent();
        content.Add(new ByteArrayContent(Encoding.UTF8.GetBytes("Public Content 2")), "file", "public.txt");
        upResponse = await _client.PostAsync($"/api/db1/files?fileId={_userId}", content, TestContext.Current.CancellationToken);
        upResponse.EnsureSuccessStatusCode();

        // 2. Download (no auth)
        _client.DefaultRequestHeaders.Authorization = null;
        downResponse = await _client.GetAsync($"/api/db1/files/{_userId}", TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.OK, downResponse.StatusCode);
        Assert.Equal("Public Content 2", await downResponse.Content.ReadAsStringAsync(TestContext.Current.CancellationToken));
    }

    [Fact]
    public async Task Delete_ShouldDelete()
    {
        // 1. Upload
        var token = GenerateToken("this_is_a_super_secret_key_for_testing_purposes_2", _userId); // Use second key
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);

        var content = new MultipartFormDataContent();
        content.Add(new ByteArrayContent(Encoding.UTF8.GetBytes("Delete Me")), "file", "del.txt");
        var upResponse = await _client.PostAsync("/api/db1/files", content, TestContext.Current.CancellationToken);
        upResponse.EnsureSuccessStatusCode();

        var json = JsonDocument.Parse(await upResponse.Content.ReadAsStringAsync(TestContext.Current.CancellationToken));
        var id = json.RootElement.GetProperty("id").GetString();

        // 2. Delete
        var delResponse = await _client.DeleteAsync($"/api/db1/files/{id}", TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.NoContent, delResponse.StatusCode);

        // 3. Verify Gone
        var getResponse = await _client.GetAsync($"/api/db1/files/{id}", TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.NotFound, getResponse.StatusCode);
    }

    [Fact]
    public async Task Delete_UploadedSameFileTwice_ShouldDelete_ButLeaveOne()
    {
        // 1. Upload
        var token = GenerateToken("this_is_a_super_secret_key_for_testing_purposes_2", _userId); // Use second key
        _client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", token);
        var contentString = "Delete Me";
        var content = new MultipartFormDataContent();
        content.Add(new ByteArrayContent(Encoding.UTF8.GetBytes(contentString)), "file", "del.txt");
        var upResponse = await _client.PostAsync("/api/db1/files", content, TestContext.Current.CancellationToken);
        upResponse.EnsureSuccessStatusCode();
        var upResponse2 = await _client.PostAsync("/api/db1/files", content, TestContext.Current.CancellationToken);
        upResponse2.EnsureSuccessStatusCode();
        var upResponse3 = await _client.PostAsync("/api/db1/files", content, TestContext.Current.CancellationToken);
        upResponse3.EnsureSuccessStatusCode();

        var json = JsonDocument.Parse(await upResponse.Content.ReadAsStringAsync(TestContext.Current.CancellationToken));
        var json2 = JsonDocument.Parse(await upResponse2.Content.ReadAsStringAsync(TestContext.Current.CancellationToken));
        var json3 = JsonDocument.Parse(await upResponse3.Content.ReadAsStringAsync(TestContext.Current.CancellationToken));
        var id1 = json.RootElement.GetProperty("id").GetString();
        var id2 = json2.RootElement.GetProperty("id").GetString();
        var id3 = json3.RootElement.GetProperty("id").GetString();

        // 2. Delete
        var delResponse2 = await _client.DeleteAsync($"/api/db1/files/{id2}", TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.NoContent, delResponse2.StatusCode);

        // 3. Verify Gone
        var getResponse2 = await _client.GetAsync($"/api/db1/files/{id2}", TestContext.Current.CancellationToken);
        Assert.Equal(HttpStatusCode.NotFound, getResponse2.StatusCode);

        // 4. Verify 1 & 3 still there
        var getResponse1 = await _client.GetAsync($"/api/db1/files/{id1}", TestContext.Current.CancellationToken);
        var getResponse3 = await _client.GetAsync($"/api/db1/files/{id3}", TestContext.Current.CancellationToken);

        Assert.Equal(HttpStatusCode.OK, getResponse1.StatusCode);
        Assert.Equal(contentString, await getResponse1.Content.ReadAsStringAsync(TestContext.Current.CancellationToken));

        Assert.Equal(HttpStatusCode.OK, getResponse3.StatusCode);
        Assert.Equal(contentString, await getResponse3.Content.ReadAsStringAsync(TestContext.Current.CancellationToken));
    }
}