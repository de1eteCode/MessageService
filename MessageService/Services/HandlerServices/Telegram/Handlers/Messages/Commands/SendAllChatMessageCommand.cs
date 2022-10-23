using MessageService.Services.HandlerServices.Telegram.Attributes;
using MessageService.Services.HelperService;
using Microsoft.EntityFrameworkCore;
using RepositoryLibrary.EFCore;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace MessageService.Services.HandlerServices.Telegram.Handlers.Messages.Commands;

/// <summary>
/// Рассылка сообщений по всем чатам
/// </summary>
[TelegramUserRole("Системный администратор")]
public class SendAllChatMessageCommand : BotCommandAction {
    private readonly IDatabaseService<DataContext> _dbService;

    public SendAllChatMessageCommand(IDatabaseService<DataContext> dbService) : base("sendallchat", "Отправка сообщения во все чаты") {
        _dbService = dbService;
    }

    public override async Task ExecuteActionAsync(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken) {
        var msgToSend = message.Text;

        if (string.IsNullOrEmpty(msgToSend)) {
            await botClient.SendTextMessageAsync(message.Chat.Id, "Сообщение для отправки было пусто. Используйте команду /sendallchat текст сообщения");
            return;
        }

        var context = _dbService.GetDBContext();
        var chats = context.Chats.Where(e => e.IsJoined).Select(e => e.ChatId);

        var chatSended = 0;

        if (chats.Any()) {
            await chats.ForEachAsync(id => {
                var msg = botClient.SendTextMessageAsync(id!, msgToSend!);
                if (msg != null) {
                    Interlocked.Increment(ref chatSended);
                }
            });
        }

        await botClient.SendTextMessageAsync(message.Chat.Id, $"Сообщение отправлено в {chatSended} чатов");
    }
}