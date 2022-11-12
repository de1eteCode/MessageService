using Telegram.Bot;
using Telegram.Bot.Types;

namespace TelegramService.Commands;

/// <summary>
/// Тестовая команда
/// Повторение сообщения, отправленного пользователем
/// </summary>
internal class ReplyMeCommand : BotCommandAction {

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