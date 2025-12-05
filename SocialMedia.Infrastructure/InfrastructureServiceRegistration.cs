
namespace SocialMedia.Infrastructure;

public static class InfrastructureServiceRegistration
{
    public static IServiceCollection AddInfrastructureServices(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<SocialMediaDbContext>(options =>
            options.UseInMemoryDatabase("SocialMediaDb"));

        services.AddDbContext<SocialMediaReadDbContext>(options =>
            options.UseInMemoryDatabase("SocialMediaReadDb"));

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

        services.AddScoped<IPostReadRepository, PostReadRepository>();
        services.AddScoped<ICommentReadRepository, CommentReadRepository>();

        services.AddScoped<IIdentityService, IdentityService>();

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

        return services;
    }
}