using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using SocialMedia.Files.API;
using System.Text;

var builder = WebApplication.CreateBuilder(args);

builder.AddServiceDefaults();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAdminSPA", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});
builder.Services.AddSwaggerGen(c =>
{
    c.SwaggerDoc("v1", new OpenApiInfo { Title = "SocialMedia.Files.API", Version = "v1" });
    c.AddSecurityDefinition("Bearer", new OpenApiSecurityScheme
    {
        Description = "JWT Authorization header using the Bearer scheme.",
        Name = "Authorization",
        In = ParameterLocation.Header,
        Type = SecuritySchemeType.Http,
        Scheme = "bearer"
    });
});

builder.Services.AddHttpContextAccessor();

builder.Services.AddDbContext<FileDbContext>((sp, options) =>
{
    var httpContextAccessor = sp.GetRequiredService<IHttpContextAccessor>();
    var config = sp.GetRequiredService<IConfiguration>();
    var context = httpContextAccessor.HttpContext;

    string shardKey = "db1"; // Default

    if (context != null && context.Request.RouteValues.TryGetValue("shardKey", out var routeVal) && routeVal?.ToString() is string val)
    {
        shardKey = val;
    }

    var connectionString = config.GetConnectionString(shardKey);
    if (string.IsNullOrEmpty(connectionString))
    {
        connectionString = config.GetConnectionString("db1");
    }
    var isTesting = builder.Environment.IsEnvironment("Testing");
    if (isTesting)
    {
        options.UseInMemoryDatabase(shardKey);
    }
    else
    {
        if (!string.IsNullOrEmpty(connectionString))
        {
            //options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString));
            options.UseSqlServer(connectionString);
        }
    }
});

builder.Services.AddScoped<IFileService, FileService>();

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer = false,
            ValidateAudience = false,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            IssuerSigningKeyResolver = (token, securityToken, kid, validationParameters) =>
            {
                var keys = builder.Configuration.GetSection("Authentication:SecretKeys").Get<List<string>>();
                return keys?.Select(k => new SymmetricSecurityKey(Encoding.UTF8.GetBytes(k))) ?? Enumerable.Empty<SecurityKey>();
            }
        };
    });

var app = builder.Build();

app.MapDefaultEndpoints();

app.UseSwagger();
app.UseSwaggerUI();

app.UseHttpsRedirection();
app.UseRouting();
app.UseCors("AllowAdminSPA");
app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

await app.RunAsync();

public partial class Program
{ }