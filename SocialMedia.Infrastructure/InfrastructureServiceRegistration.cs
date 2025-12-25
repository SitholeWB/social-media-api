namespace SocialMedia.Infrastructure;

public static class InfrastructureServiceRegistration
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        var env = services.BuildServiceProvider().GetRequiredService<IHostEnvironment>();
        var isTesting = env.IsEnvironment("Testing");
        var connectionString = configuration.GetConnectionString("WriteConnection");
        var connectionStringRead = configuration.GetConnectionString("ReadConnection");
        //services.AddDbContext<SocialMediaDbContext>(options =>
        //    options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString)));

        //services.AddDbContext<SocialMediaReadDbContext>(options =>
        //    options.UseMySql(connectionStringRead, ServerVersion.AutoDetect(connectionStringRead)));
        if (isTesting)
        {
            services.AddDbContext<SocialMediaDbContext>(options => options.UseInMemoryDatabase("write_db"));
            services.AddDbContext<SocialMediaReadDbContext>(options => options.UseInMemoryDatabase("read_db"));
        }
        else
        {
            services.AddDbContext<SocialMediaDbContext>(options => options.UseSqlServer(connectionString));
            services.AddDbContext<SocialMediaReadDbContext>(options => options.UseSqlServer(connectionStringRead));
        }

        services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
        services.AddScoped<IPostRepository, PostRepository>();
        services.AddScoped<ICommentRepository, CommentRepository>();
        services.AddScoped<ILikeRepository, LikeRepository>();
        services.AddScoped<IPollRepository, PollRepository>();
        services.AddScoped<IBlockchainService, BlockchainService>();
        services.AddScoped<IReportRepository, ReportRepository>();
        services.AddScoped<INotificationRepository, NotificationRepository>();
        services.AddScoped<IUserBlockRepository, UserBlockRepository>();
        services.AddScoped<IGroupRepository, GroupRepository>();
        services.AddScoped<IGroupMemberRepository, GroupMemberRepository>();
        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IDashboardStatsRepository, DashboardStatsRepository>();
        services.AddScoped<IUserActivityRepository, UserActivityRepository>();

        services.AddScoped<IPostReadRepository, PostReadRepository>();
        services.AddScoped<ICommentReadRepository, CommentReadRepository>();

        services.AddScoped<IIdentityService, IdentityService>();

        // Embedding Generator for Semantic Search
        services.AddSingleton<IEmbeddingGenerator, OnnxEmbeddingGenerator>();

        // Vector Store for Recommendations (Simple In-Memory Implementation)
        services.AddSingleton<SimpleInMemoryVectorStore>();
        services.AddScoped<IPostVectorService, PostVectorService>();

        // Background Event Processing
        services.AddScoped<IBackgroundEventProcessor, BackgroundEventProcessor>();
        services.AddHostedService<EventProcessorBackgroundService>();

        services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII.GetBytes(configuration.GetSection("JwtSettings:Secret").Value ?? "SuperSecretKey12345678901234567890")),
                ValidateIssuer = false,
                ValidateAudience = false
            };
        });

        // Caching Configuration
        var cacheProvider = configuration["CacheSettings:Provider"];
        if (isTesting)
        {
            cacheProvider = "use memnory";
        }
        if (string.Equals(cacheProvider, "Redis", StringComparison.OrdinalIgnoreCase))
        {
            /* services.AddStackExchangeRedisCache(options =>
             {
                 options.Configuration = configuration["CacheSettings:ConnectionString"];
             });*/
        }
        else if (string.Equals(cacheProvider, "SqlServer", StringComparison.OrdinalIgnoreCase))
        {
            services.AddDistributedSqlServerCache(options =>
            {
                options.ConnectionString = configuration["CacheSettings:ConnectionString"];
                options.SchemaName = configuration["CacheSettings:SchemaName"] ?? "dbo";
                options.TableName = configuration["CacheSettings:TableName"] ?? "DistCache";
            });
        }
        else
        {
            services.AddDistributedMemoryCache();
        }

        return services;
    }
}