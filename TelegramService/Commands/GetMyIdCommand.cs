using Telegram.Bot;
using Telegram.Bot.Types;

namespace TelegramService.Commands;

internal class GetMyIdCommand : BotCommandAction {

    public GetMyIdCommand() : base("getmyid", "Получение идентификатора вашей учетной записи Telegram") {
    }

    public override async Task ExecuteActionAsync(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken) {
        await botClient.SendTextMessageAsync(message.Chat.Id, $"Ваш идентификатор: " + message.From!.Id, cancellationToken: cancellationToken);
    }
}