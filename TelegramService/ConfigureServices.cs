using Application.Common.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot.Types;
using TelegramService.Commands;
using TelegramService.Handlers;
using TelegramService.Interfaces;
using TelegramService.PassiveCommands;
using TelegramService.Services;

namespace TelegramService;

public static class ConfigureServices {

    /// <summary>
    /// Добавление команды боту
    /// </summary>
    private static IServiceCollection AddTelegramCommand<T>(this IServiceCollection services)
        where T : BotCommandAction =>
        services.AddTransient<BotCommandAction, T>();

    private static IServiceCollection AddTelegramHandler<T, THandler>(this IServiceCollection services)
        where THandler : class, IUpdateHandler<T> =>
        services.AddScoped<IUpdateHandler<T>, THandler>();

    public static IServiceCollection AddTelegramHostedService(this IServiceCollection services) => services
        // Identity
        .AddTransient<ICurrentUserService, TelegramCurrentUserService>()
        .AddTransient<IIdentityService, TelegramIdentityService>()

        // Handlers
        .AddTelegramHandler<Message, MessageHandler>()
        .AddTelegramHandler<ChatMemberUpdated, MyChatMemberHandler>()

        // Commands, apply for message handler
        .AddTelegramCommand<ReplyMeCommand>()
        .AddTelegramCommand<SendAllChatMessageCommand>()
        .AddTelegramCommand<GetChatsInfoCommand>()
        .AddTelegramCommand<GetGroupsInfoCommand>()
        .AddTelegramCommand<GetGroupInfoCommand>()
        .AddTelegramCommand<LeaveChatByIdCommand>()
        .AddTelegramCommand<AddUserCommand>()
        .AddTelegramCommand<ChangeUserCommand>()
        .AddTelegramCommand<AddGroupCommand>()
        .AddTelegramCommand<AddChatToGroupCommand>()
        .AddTelegramCommand<SendAllChatByGroupCommand>()
        .AddTelegramCommand<RemoveChatFromGroupCommand>()
        .AddTelegramCommand<GetMyIdCommand>()
        .AddTelegramCommand<StressTestCommand>()
        .AddTelegramCommand<AddUserToGroupCommand>()
        .AddTelegramCommand<SendChatById>()

        // Chat members passive executeon
        .AddTransient<ForgetChat>()
        .AddTransient<RememberChat>()

        .AddTransient<IWhoIam, TelegramHostedService>(service => service.GetRequiredService<TelegramHostedService>())
        //.AddTransient<ITelegramSenderMessage, TelegramService>(service => (TelegramService)service.GetRequiredService<ITelegramService>())

        // service
        .AddSingleton<TelegramHostedService>()
        .AddHostedService<TelegramHostedService>(service => service.GetRequiredService<TelegramHostedService>());
}