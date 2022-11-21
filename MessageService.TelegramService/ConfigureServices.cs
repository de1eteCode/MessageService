using System.Reflection;
using MediatR;
using MessageService.TelegramService;
using MessageService.TelegramService.Commands;
using MessageService.TelegramService.Common.Behaviours;
using MessageService.TelegramService.Common.Interfaces;
using MessageService.TelegramService.Common.Models;
using MessageService.TelegramService.Handlers;
using Microsoft.Extensions.Options;
using Telegram.BotAPI;
using Telegram.BotAPI.AvailableTypes;

namespace Microsoft.Extensions.DependencyInjection;

public static class ConfigureServices {

    public static IServiceCollection AddTelegramService(this IServiceCollection services) {
        services.AddMediatR(Assembly.GetExecutingAssembly());

        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(UnhandledTelegramExceptionBehaviour<,>));

        // telegram handlers
        services.AddHandler<Message, MessageHandler>();
        services.AddHandler<ChatMemberUpdated, ChatMemberUpdatedHandler>();

        // telegram commands
        services.AddCommand<ReplyMeCommand>();

        services.AddPassiveCommand<ForgetChatCommand>();

        services.AddSingleton(services => {
            var config = services.GetRequiredService<IOptionsMonitor<TelegramBotSettings>>().CurrentValue;
            return new BotClient(config.Token, config.IgnoreBotExceptions);
        });

        services.AddHostedService<TelegramServiceHost>();

        return services;
    }

    private static IServiceCollection AddCommand<TCommand>(this IServiceCollection services)
        where TCommand : class, ITelegramRequest {
        return services.AddTransient<ITelegramRequest, TCommand>();
    }

    private static IServiceCollection AddPassiveCommand<TCommand>(this IServiceCollection services)
        where TCommand : class, ITelegramPassiveRequest {
        return services.AddTransient<ITelegramPassiveRequest, TCommand>();
    }

    private static IServiceCollection AddHandler<TUpdate, THandler>(this IServiceCollection services)
        where THandler : class, ITelegramUpdateHandler<TUpdate> {
        return services.AddTransient(typeof(ITelegramUpdateHandler<TUpdate>), typeof(THandler));
    }
}