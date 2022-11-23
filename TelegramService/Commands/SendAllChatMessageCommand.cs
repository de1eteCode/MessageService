using Application.Chats.Queries;
using MediatR;
using Telegram.Bot;
using Telegram.Bot.Types;
using TelegramService.Attributes;

namespace TelegramService.Commands;

/// <summary>
/// Рассылка сообщений по всем чатам
/// </summary>
[UserRole("Системный администратор")]
internal class SendAllChatMessageCommand : BotCommandAction {
    private readonly IMediator _mediator;

    public SendAllChatMessageCommand(IMediator mediator) : base("sendallchat", "Отправка сообщения во все чаты") {
        _mediator = mediator;
    }

    public override async Task ExecuteActionAsync(ITelegramBotClient botClient, Message message, CancellationToken cancellationToken) {
        var msgToSend = message.Text;

        if (string.IsNullOrEmpty(msgToSend)) {
            await botClient.SendTextMessageAsync(message.Chat.Id, "Сообщение для отправки было пусто. Используйте команду /sendallchat текст сообщения");
            return;
        }

        var chats = await _mediator.Send(new GetChatsCommand());

        var chatSended = 0;

        var tasksToSend = chats.Where(e => e.IsJoined).Select(e => Task.Run(async () => {
            var msg = await botClient.SendTextMessageAsync(e.TelegramChatId, msgToSend!);
            if (msg != null) {
                Interlocked.Increment(ref chatSended);
            }
        }));

        await Task.WhenAll(tasksToSend);

        await botClient.SendTextMessageAsync(message.Chat.Id, $"Сообщение отправлено в {chatSended} чатов");
    }
}