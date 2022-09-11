using MessageService.Services.HandlerServices.TelegramService;
using MessageService.Services.HandlerServices.TelegramService.Handlers;
using MessageService.Services.HandlerServices.TelegramService.Handlers.Messages.ChatMembers;
using MessageService.Services.HandlerServices.TelegramService.Handlers.Messages.Commands;
using Telegram.Bot.Types;

namespace MessageService.Services.HandlerServices;

public static class TelegramServiceExtension {

    /// <summary>
    /// Добавление команды боту
    /// </summary>
    private static IServiceCollection AddTelegramCommand<T>(this IServiceCollection services) where T : BotCommandAction =>
        services.AddTransient<BotCommandAction, T>();

    private static IServiceCollection AddTelegramHandler<T, THandler>(this IServiceCollection services)
        where THandler : class, IUpdateHandler<T> {
        return services.AddTransient<IUpdateHandler<T>, THandler>();
    }

    public static IServiceCollection AddTelegramHandler(this IServiceCollection services) => services
        // Handlers
        .AddTelegramHandler<Message, MessageHandler>()

        // Commands, apply for message handler
        .AddTelegramCommand<StartCommand>()
        .AddTelegramCommand<ReplyMeCommand>()

        // Chat members passive executeon
        .AddTransient<ForgetChat>()
        .AddTransient<RememberChat>()

        // service
        .AddSingleton<ITelegramHandlerService, TelegramHandlerService>()
        .AddHostedService(service => (TelegramHandlerService)service.GetRequiredService<ITelegramHandlerService>());
}