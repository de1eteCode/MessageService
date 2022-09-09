using MessageService.Datas;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace MessageService.Services.HandlerServices.TelegramService.Commands;

public class ReplyMeCommand : BotCommandAction {

    public ReplyMeCommand() : base("replyme", "Отправь сообщение и бот его повторит") {
    }

    public override async void ExecuteAction(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken) {
        var msg = update.Message.Text;

        if (string.IsNullOrEmpty(msg)) {
            msg = "Вы ничего не отправили :с";
        }

        await botClient.SendTextMessageAsync(update.Message.Chat.Id, msg);
    }
}