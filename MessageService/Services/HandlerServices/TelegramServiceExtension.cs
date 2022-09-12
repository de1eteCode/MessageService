﻿using MessageService.Services.HandlerServices.Telegram;
using MessageService.Services.HandlerServices.Telegram.AttributeValidators;
using MessageService.Services.HandlerServices.Telegram.Handlers;
using MessageService.Services.HandlerServices.Telegram.Handlers.Messages;
using MessageService.Services.HandlerServices.Telegram.Handlers.Messages.ChatMembers;
using MessageService.Services.HandlerServices.Telegram.Handlers.Messages.Commands;
using Telegram.Bot.Types;

namespace MessageService.Services.HandlerServices;

public static class TelegramServiceExtension {

    /// <summary>
    /// Добавление команды боту
    /// </summary>
    private static IServiceCollection AddTelegramCommand<T>(this IServiceCollection services)
        where T : BotCommandAction =>
        services.AddTransient<BotCommandAction, T>();

    private static IServiceCollection AddTelegramHandler<T, THandler>(this IServiceCollection services)
        where THandler : class, IUpdateHandler<T> =>
        services.AddTransient<IUpdateHandler<T>, THandler>();

    private static IServiceCollection AddTelegramValidator<T>(this IServiceCollection services)
        where T : class, ITelegramValidator =>
        services.AddTransient<ITelegramValidator, T>();

    public static IServiceCollection AddTelegramHandler(this IServiceCollection services) => services
        // Handlers
        .AddTelegramHandler<Message, MessageHandler>()

        // Validators
        .AddTelegramValidator<TelegramLoginValidator>()
        .AddTelegramValidator<TelegramUserRoleValidator>()

        // Commands, apply for message handler
        .AddTelegramCommand<StartCommand>()
        .AddTelegramCommand<ReplyMeCommand>()
        .AddTelegramCommand<SendAllChatMessageCommand>()
        .AddTelegramCommand<GetChatsInfoCommand>()
        .AddTelegramCommand<LeaveChatByIdCommand>()
        .AddTelegramCommand<AddUserCommand>()

        // Chat members passive executeon
        .AddTransient<ForgetChat>()
        .AddTransient<RememberChat>()

        .AddTransient<IWhoIam, TelegramService>(service => (TelegramService)service.GetRequiredService<ITelegramService>())
        .AddTransient<ITelegramSenderMessage, TelegramService>(service => (TelegramService)service.GetRequiredService<ITelegramService>())

        // service
        .AddSingleton<ITelegramService, TelegramService>()
        .AddHostedService(service => (TelegramService)service.GetRequiredService<ITelegramService>());
}