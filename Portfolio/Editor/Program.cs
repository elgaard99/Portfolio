using Editor.Components;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Distributed;
using SharedLib.Data;
using SharedLib.Services;
using StackExchange.Redis;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// Add Bootstrap
builder.Services.AddBlazorBootstrap();

// Add Logging
builder.Services.AddLogging();

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

builder.Services.AddScoped<IBlogPostService, BlogPostService>();
builder.Services.AddScoped<IPhotoService, PhotoService>();

// Add HttpClient
builder.Services.AddHttpClient();

// Add MinIO
builder.Services.AddScoped<MinioService>(provider =>
    new MinioService(
        logger: provider.GetRequiredService<ILogger<MinioService>>(),
        httpClientFactory: provider.GetRequiredService<IHttpClientFactory>(),
        cache: provider.GetRequiredService<IDistributedCache>(),
        endpoint: builder.Configuration["MinIo:Endpoint"],
        accessKey: builder.Configuration["MinIo:AccessKey"],
        secretKey: builder.Configuration["MinIo:SecretKey"]
    )
);

var app = builder.Build();

// Configure the HTTP request pipeline.
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
}

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();