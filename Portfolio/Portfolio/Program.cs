using Microsoft.AspNetCore.Components;
using Portfolio.Components;

namespace Portfolio;

public class Program
{
    public static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        
        builder.Services.AddBlazorBootstrap();
        builder.Services.AddLogging();
        
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