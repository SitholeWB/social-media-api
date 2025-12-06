var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.AddServiceDefaults();
builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddExceptionHandler<SocialMedia.API.Middleware.GlobalExceptionHandler>();
builder.Services.AddProblemDetails();

builder.Services.AddApiVersioning(options =>
{
    options.DefaultApiVersion = new Asp.Versioning.ApiVersion(1, 0);
    options.AssumeDefaultVersionWhenUnspecified = true;
    options.ReportApiVersions = true;
})
.AddMvc()
.AddApiExplorer(options =>
{
    options.GroupNameFormat = "'v'VVV";
    options.SubstituteApiVersionInUrl = true;
});

// Add CORS policy for Admin SPA
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowAdminSPA", policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyHeader()
              .AllowAnyMethod();
    });
});

// Add Application and Infrastructure services
builder.Services.AddApplicationServices();
builder.Services.AddInfrastructureServices(builder.Configuration);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseExceptionHandler();
app.UseHttpsRedirection();

// Serve static files from wwwroot
app.UseDefaultFiles();
app.UseStaticFiles();

// Enable CORS
app.UseCors("AllowAdminSPA");

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

// SPA fallback - serve index.html for all non-API routes
//app.MapFallbackToFile("index.html");

await app.RunAsync();