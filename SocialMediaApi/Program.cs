using Microsoft.AspNetCore.Diagnostics;
using Microsoft.EntityFrameworkCore;
using SocialMediaApi.Data;
using SocialMediaApi.Domain.Exceptions;
using SocialMediaApi.Interfaces;
using SocialMediaApi.Logic.Services;
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

            // Add services to the container.

            builder.Services.AddControllers();
            builder.Services.AddScoped<IAuthService, AuthService>();
            builder.Services.AddScoped<IGroupService, GroupService>();
            builder.Services.AddNotificationHandlers();
            builder.Services.AddEventSubscriptions();

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();

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