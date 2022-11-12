using Application.Common.Interfaces;
using Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using TelegramService;
using TelegramService.Models;
using WebUI.Data;

namespace WebUI;

public class Program {

    public static void Main(string[] args) {
        var builder = WebApplication.CreateBuilder(args);

        // Telegram settings
        builder.Services.Configure<TelegramSettings>(builder.Configuration.GetSection("TelgeramSettings"));

        // Infrastructure: EF Core
        builder.Services.AddDbContext<IDataContext, DataContext>(options => {
            options.UseLazyLoadingProxies();
            options.UseNpgsql(builder.Configuration.GetConnectionString("psql"));

#if DEBUG
            options.EnableSensitiveDataLogging();
#endif
        },
        contextLifetime: ServiceLifetime.Transient);

        // Handlers
        builder.Services.AddTelegramHostedService();

        // Add services to the container.
        builder.Services.AddRazorPages();
        builder.Services.AddServerSideBlazor();
        builder.Services.AddSingleton<WeatherForecastService>();

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (!app.Environment.IsDevelopment()) {
            app.UseExceptionHandler("/Error");
            // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
            app.UseHsts();
        }

        app.UseHttpsRedirection();

        app.UseStaticFiles();

        app.UseRouting();

        app.MapBlazorHub();
        app.MapFallbackToPage("/_Host");

        app.Run();
    }
}