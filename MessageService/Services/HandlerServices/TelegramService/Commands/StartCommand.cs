using Telegram.Bot;
using Telegram.Bot.Types;

namespace MessageService.Services.HandlerServices.TelegramService.Commands;

public class StartCommand : BotCommandAction {

    public StartCommand()
        : base("start", "Инициализация бота") {
    }

    public override void ExecuteAction(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken) {
        throw new NotImplementedException();
    }
}