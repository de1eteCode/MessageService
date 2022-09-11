using MessageService.Services.HandlerServices.Telegram;
using MessageService.Services.HandlerServices.Telegram.Handlers;
using MessageService.Services.HandlerServices.Telegram.Handlers.Messages.ChatMembers;
using MessageService.Services.HandlerServices.Telegram.Handlers.Messages.Commands;
using Microsoft.Extensions.DependencyInjection;
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
        .AddTelegramCommand<SendAllChatMessageCommand>()

        // Chat members passive executeon
        .AddTransient<ForgetChat>()
        .AddTransient<RememberChat>()

        .AddTransient<IWhoIam, TelegramService>(service => (TelegramService)service.GetRequiredService<ITelegramService>())
        .AddTransient<ITelegramSenderMessage, TelegramService>(service => (TelegramService)service.GetRequiredService<ITelegramService>())

        // service
        .AddSingleton<ITelegramService, TelegramService>()
        .AddHostedService(service => (TelegramService)service.GetRequiredService<ITelegramService>());
}