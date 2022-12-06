using System.Reflection;
using MediatR;
using MessageService.TelegramService;
using MessageService.TelegramService.Commands;
using MessageService.TelegramService.Common.AttributeValidators;
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

        // telegram validators
        services.AddTelegramValidator<UserNameValidator>();
        services.AddTelegramValidator<UserRoleValidator>();

        // telegram commands
        services.AddCommand<AddChatToGroupCommand, AddChatToGroupCommandParamsBuilder>();
        services.AddCommand<AddGroupCommand, AddGroupCommandParamsBuilder>();
        services.AddCommand<AddUserCommand, AddUserCommandParamsBuilder>();
        services.AddCommand<AddUserToGroupCommand, AddUserToGroupCommandParamsBuilder>();
        services.AddCommand<ChangeUserCommand, ChangeUserCommandParamsBuilder>();
        services.AddCommand<GetChatsInfoCommand, GetChatsInfoCommandParamsBuilder>();
        services.AddCommand<GetGroupInfoCommand, GetGroupInfoCommandParamsBuilder>();
        services.AddCommand<GetGroupsInfoCommand, GetGroupsInfoCommandParamsBuilder>();
        services.AddCommand<GetMyIdCommand, GetMyIdCommandParamsBuilder>();
        services.AddCommand<LeaveChatByIdCommand, LeaveChatByIdCommandParamsBuilder>();
        services.AddCommand<RemoveChatFromGroupCommand, RemoveChatFromGroupCommandParamsBuilder>();
        services.AddCommand<ReplyMeCommand, ReplyMeCommandParamsBuilder>();
        services.AddCommand<SendAllChatByGroupCommand, SendAllChatByGroupCommandParamsBuilder>();
        services.AddCommand<SendAllChatMessageCommand, SendAllChatMessageCommandParamsBuilder>();
        services.AddCommand<SendChatByIdCommand, SendChatByIdCommandParamsBuilder>();

        services.AddPassiveCommand<ForgetChatCommand, ForgetChatCommandParamsBuilder>();
        services.AddPassiveCommand<RememberChatCommand, RememberChatCommandParamsBuilder>();

        services.AddSingleton(services => {
            var config = services.GetRequiredService<IOptionsMonitor<TelegramBotSettings>>().CurrentValue;
            return new BotClient(config.Token, config.IgnoreBotExceptions);
        });

        services.AddHostedService<TelegramServiceHost>();

        return services;
    }

    /// <summary>
    /// Добавление команд для пользователей
    /// </summary>
    /// <typeparam name="TCommand">Тип команды</typeparam>
    /// <typeparam name="TParamsBuilder">Тип конструктора параметров команды</typeparam>
    /// <param name="services">Коллекция сервисов</param>
    /// <returns><paramref name="services"/></returns>
    private static IServiceCollection AddCommand<TCommand, TParamsBuilder>(this IServiceCollection services)
        where TCommand : class, ITelegramRequest
        where TParamsBuilder : class, ITelegramRequestParamsBuilder<TCommand> => services
        .AddTransient<ITelegramRequest, TCommand>()
        .AddTransient<ITelegramRequestParamsBuilder<TCommand>, TParamsBuilder>();

    /// <summary>
    /// Добавление пассивных команд (не доступные пользователям)
    /// </summary>
    /// <typeparam name="TCommand">Тип пассивной команды</typeparam>
    /// <typeparam name="TParamsBuilder">Тип конструктора параметров команды</typeparam>
    /// <param name="services">Коллекция сервисов</param>
    /// <returns><paramref name="services"/></returns>
    private static IServiceCollection AddPassiveCommand<TCommand, TParamsBuilder>(this IServiceCollection services)
        where TCommand : class, ITelegramPassiveRequest
        where TParamsBuilder : class, ITelegramPassiveRequestParamsBuilder<TCommand> => services
        .AddTransient<ITelegramPassiveRequest, TCommand>()
        .AddTransient<ITelegramPassiveRequestParamsBuilder<TCommand>, TParamsBuilder>();

    private static IServiceCollection AddHandler<TUpdate, THandler>(this IServiceCollection services)
        where THandler : class, ITelegramUpdateHandler<TUpdate> =>
        services.AddTransient(typeof(ITelegramUpdateHandler<TUpdate>), typeof(THandler));

    private static IServiceCollection AddTelegramValidator<T>(this IServiceCollection services)
        where T : class, IValidator =>
        services.AddTransient<IValidator, T>();
}