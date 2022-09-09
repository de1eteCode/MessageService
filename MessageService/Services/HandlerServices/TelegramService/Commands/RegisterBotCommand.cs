using MessageService.Datas;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace MessageService.Services.HandlerServices.TelegramService.Commands {

    /// <summary>
    /// Регистрация бота в чате
    /// </summary>
    public class RegisterBotCommand : BotCommandActionDB {

        public RegisterBotCommand(IServiceScopeFactory serviceScopeFactory) : base(serviceScopeFactory, "register", "Регистрация ") {
        }

        public override void ExecuteAction(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken, DataContext dataContext) {
            throw new NotImplementedException();
        }
    }
}