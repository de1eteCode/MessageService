using MessageService.Services.HandlerServices.TelegramService;
using MessageService.Services.HandlerServices.TelegramService.Commands;
using MessageService.Services.HandlerServices.TelegramService.Handlers;
using Telegram.Bot.Types;

namespace MessageService.Services.HandlerServices {

    public static class TelegramServiceExtension {

        public static IServiceCollection AddTelegramCommand<T>(this IServiceCollection services)
            where T : BotCommandAction {
            services.AddTransient<BotCommandAction, T>();
            return services;
        }

        public static IServiceCollection AddAllTelegramCommands(this IServiceCollection services) {
            return services
                .AddTelegramCommand<StartCommand>()
                .AddTelegramCommand<ReplyMeCommand>()
                .AddTelegramCommand<AddUserCommand>();
        }

        public static IServiceCollection AddAllTelegramUpdateHandlers(this IServiceCollection services) {
            return services.AddTransient<CommandHandler>();
        }

        public static IServiceCollection AddTelegramHandler(this IServiceCollection services) {
            services.AddAllTelegramUpdateHandlers();
            services.AddSingleton<ITelegramHandlerService, TelegramHandlerService>();
            services.AddHostedService<TelegramHandlerService>();
            return services;
        }
    }
}