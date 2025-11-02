using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Portfolio.Services;
using Portfolio.Components;
using SharedLib.Data;
using SharedLib.Services;
using StackExchange.Redis;

namespace Portfolio;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        // Add yaml service
        string[] supportedLanguages = { "en", "da", "fr" };
        builder.Services.AddScoped<YamlLocalizationService>(provider => 
            new YamlLocalizationService(
                webHost: provider.GetRequiredService<IWebHostEnvironment>(),
                logger: provider.GetRequiredService<ILogger<YamlLocalizationService>>(),
                supportedLanguages: supportedLanguages,
                defaultLanguage: "da"
                ));
        
        // Add services to the container.
        builder.Services.AddRazorComponents()
            .AddInteractiveServerComponents();
        
        // Add redis
        var configuration = builder.Configuration;
        var redisConnString = configuration.GetConnectionString("Redis")
            ?? throw new NullReferenceException("Redis connection string not found");
        
        var redis = ConnectionMultiplexer.Connect(redisConnString);
        builder.Services.AddDataProtection()
            .PersistKeysToStackExchangeRedis(redis, "DataProtection-Keys")
            .SetApplicationName("Portfolio");
        
        builder.Services.AddStackExchangeRedisCache(options =>
        {
            options.Configuration = redis.Configuration;
            options.InstanceName = "Portfolio"; // optional namespace prefix
        });
        
        // Add postgres
        var pgConnectionString = configuration.GetConnectionString("Postgres") 
            ?? throw new NullReferenceException("Postgres connection string not found");
        
        builder.Services.AddPooledDbContextFactory<AppDbContext>(options =>
            options.UseNpgsql(pgConnectionString));
        
        // Add services
        builder.Services.AddScoped<IBlogPostService, BlogPostService>();
        builder.Services.AddBlazorBootstrap();
        builder.Services.AddLogging();
        
        // Add HttpClient
        builder.Services.AddHttpClient();
        
        // Add MinIO
        builder.Services.AddScoped<MinioService>(provider =>
            new MinioService(
                logger: provider.GetRequiredService<ILogger<MinioService>>(),
                httpClientFactory: provider.GetRequiredService<IHttpClientFactory>(),
                endpoint: builder.Configuration["MinIo:Endpoint"] ?? throw new NullReferenceException("MinIo:Endpoint"),
                accessKey: builder.Configuration["MinIo:AccessKey"] ?? throw new NullReferenceException("MinIo:AccessKey"),
                secretKey: builder.Configuration["MinIo:SecretKey"]  ?? throw new NullReferenceException("MinIo:SecretKey")
            )
        );

        var app = builder.Build();
        
        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }
        
        app.UseAntiforgery();

        app.MapStaticAssets();
        app.MapRazorComponents<App>()
            .AddInteractiveServerRenderMode();

        app.Run();
    }
}