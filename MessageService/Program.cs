using MessageService.Datas;
using MessageService.Services.HandlerServices;
using MessageService.Services.HelperService;
using Microsoft.EntityFrameworkCore;

namespace MessageService;

public class Program {

    public static void Main(string[] args) {
        var builder = WebApplication.CreateBuilder(args);
        // Add services to the container.
        builder.Services.AddDbContext<DataContext>(options => {
            options.UseLazyLoadingProxies();
            options.UseNpgsql(builder.Configuration.GetConnectionString("psql"));
        });
        builder.Services.AddRazorPages();
        builder.Services.AddServerSideBlazor();
        builder.Services.AddSingleton<Data.WeatherForecastService>();

        builder.Services.AddTransient<IDatabaseService<DataContext>, ScopeDatabaseService<DataContext>>();

        // Telegram service
        builder.Services.AddTelegramHandler();

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