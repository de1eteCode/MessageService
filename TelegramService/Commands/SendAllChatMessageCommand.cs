using Application.Common.Interfaces;
using Microsoft.EntityFrameworkCore;
using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramService.Attributes;

namespace TelegramService.Commands;

/// <summary>
/// Рассылка сообщений по всем чатам
/// </summary>
[UserRole("Системный администратор")]
internal class SendAllChatMessageCommand : BotCommandAction {
    private readonly IDataContext _context;

    public SendAllChatMessageCommand(IDataContext context) : base("sendallchat", "Отправка сообщения во все чаты") {
        _context = context;
    }

    public override async Task ExecuteActionAsync(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken) {
        var msgToSend = message.Text;

        if (string.IsNullOrEmpty(msgToSend)) {
            await botClient.SendTextMessageAsync(message.Chat.Id, "Сообщение для отправки было пусто. Используйте команду /sendallchat текст сообщения");
            return;
        }

        var chats = _context.Chats.Where(e => e.IsJoined).Select(e => e.TelegramChatId);

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