using Infrastructure;
using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot.Types;
using TelegramService.AttributeValidators;
using TelegramService.Commands;
using TelegramService.Handlers;
using TelegramService.Interfaces;
using TelegramService.PassiveCommands;

namespace TelegramService;

public static class ServiceExtensions {

    /// <summary>
    /// Добавление команды боту
    /// </summary>
    private static IServiceCollection AddTelegramCommand<T>(this IServiceCollection services)
        where T : BotCommandAction =>
        services.AddTransient<BotCommandAction, T>();

    private static IServiceCollection AddTelegramHandler<T, THandler>(this IServiceCollection services)
        where THandler : class, IUpdateHandler<T> =>
        services.AddScoped<IUpdateHandler<T>, THandler>();

    private static IServiceCollection AddTelegramValidator<T>(this IServiceCollection services)
        where T : class, IValidator =>
        services.AddScoped<IValidator, T>();

    public static IServiceCollection AddTelegramHostedService(this IServiceCollection services) => services
        // Handlers
        .AddTelegramHandler<Message, MessageHandler>()
        .AddTelegramHandler<ChatMemberUpdated, MyChatMemberHandler>()

        // Validators
        .AddTelegramValidator<LoginValidator>()
        .AddTelegramValidator<UserRoleValidator>()

        // Commands, apply for message handler
        .AddTelegramCommand<ReplyMeCommand>()
        .AddTelegramCommand<SendAllChatMessageCommand>()
        .AddTelegramCommand<GetChatsInfoCommand>()
        .AddTelegramCommand<GetGroupsInfoCommand>()
        .AddTelegramCommand<GetGroupInfoCommand>()
        .AddTelegramCommand<LeaveChatByIdCommand>()
        .AddTelegramCommand<LeaveChatByOwnerNameCommand>()
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