using MessageService.Data;
using MessageService.Datas;
using MessageService.Services.HandlerServices.TelegramService;
using MessageService.Services.HandlerServices.TelegramService.Commands;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.EntityFrameworkCore;

namespace MessageService;

public class Program {

    public static void Main(string[] args) {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        builder.Services.AddDbContext<DataContext>(options => {
            options.UseNpgsql(builder.Configuration.GetConnectionString("psql"));
        });

        builder.Services.AddRazorPages();
        builder.Services.AddServerSideBlazor();
        builder.Services.AddSingleton<WeatherForecastService>();

        // Telegram commands
        builder.Services.AddTransient<BotCommandAction, StartCommand>();
        builder.Services.AddTransient<BotCommandAction, ReplyMeCommand>();

        builder.Services.AddScoped<ITelegramHandlerService, TelegramHandlerService>();
        builder.Services.AddHostedService<TelegramHandlerService>();

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