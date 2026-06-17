using Server.Data;
using Server.Services;
using Server.Core.Interfaces;

using System;
using System.Text;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        var connectionString = builder.Configuration.GetConnectionString("DefaultConnection");

        builder.Services.AddControllers();
        builder.Services.AddOpenApi();
        builder.Services.AddDbContext<AppDbContext>(options =>
        {
            options.UseMySql(connectionString, ServerVersion.AutoDetect(connectionString))
                   .LogTo(Console.WriteLine, Microsoft.Extensions.Logging.LogLevel.Information);
        });

        builder.Services.AddScoped<ITaskService, TaskService>()
                        .AddScoped<ICategoryService, CategoryService>()
                        .AddScoped<IAuthService, AuthService>();

        builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
            .AddJwtBearer(options =>
            {
                options.TokenValidationParameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey
                        (Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:SecretKey"]
                            ?? throw new InvalidOperationException(
                                "JWT Secret Key is completely missing from appsettings.json!"
                            ))),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero
                };
            });

        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowAngularDevClient", policy =>
            {
                policy.WithOrigins("http://localhost:4200")
                      .AllowAnyHeader()
                      .AllowAnyMethod();
            });
        });

        builder.Services.AddControllers()
            .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Converters
                        .Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
                });

        var app = builder.Build();

        if (app.Environment.IsDevelopment())
        {
            app.MapOpenApi();
        }

        app.UseCors("AllowAngularDevClient");
        app.UseHttpsRedirection();
        app.UseAuthentication();
        app.UseAuthorization();
        app.MapControllers();

        app.Run();
    }
}
