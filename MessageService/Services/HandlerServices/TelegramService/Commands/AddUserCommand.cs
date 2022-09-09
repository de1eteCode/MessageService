using MessageService.Datas;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace MessageService.Services.HandlerServices.TelegramService.Commands {

    public class AddUserCommand : BotCommandActionDB {

        public AddUserCommand(IServiceScopeFactory serviceScopeFactory) : base(serviceScopeFactory, "adduser", "Добавление пользователя") {
        }

        public override void ExecuteAction(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken, DataContext dataContext) {
            botClient.SendTextMessageAsync(update.Message.Chat.Id, "Пользователь добавлен");
        }
    }
}