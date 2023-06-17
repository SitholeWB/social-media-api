using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using SocialMediaApi.Data;
using SocialMediaApi.Domain.Exceptions;
using SocialMediaApi.Interfaces;
using SocialMediaApi.Interfaces.UnitOfWork;
using SocialMediaApi.Logic.Services;
using SocialMediaApi.Logic.UnitOfWork;
using System.Text.Json;

namespace SocialMediaApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Services.AddDbContext<SocialMediaApiDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("WebApiContext") ?? throw new InvalidOperationException("Connection string 'WebApiContext' not found.")));

            builder.Services.AddCors(options =>
            {
                options.AddPolicy("AllowAll",
                    builder =>
                    {
                        builder
                        .AllowAnyOrigin()
                        .AllowAnyMethod()
                        .AllowAnyHeader();
                    });
            });

            // Add services to the container.

            builder.Services.AddControllers();
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IConfigService, ConfigService>();
            builder.Services.AddScoped<IGroupService, GroupService>();
            builder.Services.AddScoped<IPostService, PostService>();
            builder.Services.AddScoped<ICommentService, CommentService>();
            builder.Services.AddScoped<IActivePostService, ActivePostService>();
            builder.Services.AddScoped<IEntityDetailsService, EntityDetailsService>();
            builder.Services.AddScoped<IPostReactionService, PostReactionService>();
            builder.Services.AddScoped<ICommentReactionService, CommentReactionService>();
            builder.Services.AddScoped<IUserDetailsService, UserDetailsService>();
            builder.Services.AddScoped<IUserPostService, UserPostService>();
            builder.Services.AddScoped<IUserService, UserService>();
            builder.Services.AddScoped<IPostUnitOfWork, PostUnitOfWork>();
            builder.Services.AddNotificationHandlers();
            builder.Services.AddEventSubscriptions();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();
            app.UseCors("AllowAll");
            app.UseSwagger();
            app.UseSwaggerUI();

            app.UseExceptionHandler(a => a.Run(async context =>
            {
                var exceptionHandlerPathFeature = context.Features.Get<IExceptionHandlerPathFeature>();
                var exception = exceptionHandlerPathFeature?.Error;
                if (exception is SocialMediaException)
                {
                    var result = JsonSerializer.Serialize(new { error = exception.Message });
                    context.Response.ContentType = "application/json";
                    context.Response.StatusCode = StatusCodes.Status400BadRequest;
                    await context.Response.WriteAsync(result);
                }
            }));

            app.UseHttpsRedirection();

            app.UseAuthorization();

            app.MapControllers();

            app.Run();
        }
    }
}