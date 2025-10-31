using Editor.Components;
using Microsoft.EntityFrameworkCore;
using SharedLib.Data;
using SharedLib.Services;

var builder = WebApplication.CreateBuilder(args);

// Add Bootstrap
builder.Services.AddBlazorBootstrap();

// Add Logging
builder.Services.AddLogging();

// Add postgres db
var pgConnectionString = builder.Configuration.GetConnectionString("Postgres")
                         ?? throw new NullReferenceException("Postgres");
        
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseNpgsql(pgConnectionString));

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddScoped<IBlogPostService, BlogPostService>();

// Add MinIO
builder.Services.AddScoped<MinioService>(provider =>
    new MinioService(
        logger: provider.GetRequiredService<ILogger<MinioService>>(),
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