using Telegram.Bot;
using Telegram.Bot.Types;

namespace MessageService.Services.HandlerServices.Telegram.Handlers.Messages.Commands;

/// <summary>
/// Тестовая команда
/// Повторение сообщения, отправленного пользователем
/// </summary>
public class ReplyMeCommand : BotCommandAction {

    public ReplyMeCommand() : base("replyme", "Отправь сообщение и бот его повторит") {
    }

    public override async Task ExecuteActionAsync(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken) {
        var msg = message.Text;

        if (string.IsNullOrEmpty(msg)) {
            msg = "Вы ничего не отправили :с";
        }

        await botClient.SendTextMessageAsync(message.Chat.Id, msg);
    }
}